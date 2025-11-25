using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using projectPart1.Models;
using projectPart1.Data;
using projectPart1.Services;
using System.Data;

namespace projectPart1.Pages
{
    public class AddArtworkModel : PageModel
    {
        private readonly DatabaseHelper dbHelper;
        private readonly FileUploadService fileUploadService;
        private readonly ILogger<AddArtworkModel> logger;

        public AddArtworkModel(DatabaseHelper dbHelper, FileUploadService fileUploadService, ILogger<AddArtworkModel> logger)
        {
            this.dbHelper = dbHelper;
            this.fileUploadService = fileUploadService;
            this.logger = logger;
        }

        [BindProperty]
        public Artwork NewArtwork { get; set; } = new Artwork();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public List<Artist> AvailableArtists { get; set; } = new List<Artist>();

        public async Task OnGetAsync()
        {
            // Load available artists for dropdown
            await LoadAvailableArtistsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Model state is invalid for new artwork");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Additional validation
                if (NewArtwork.Price < 0)
                {
                    ModelState.AddModelError("NewArtwork.Price", "Price cannot be negative");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                if (NewArtwork.Price > 999999)
                {
                    ModelState.AddModelError("NewArtwork.Price", "Price is too high");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Validate ArtistId exists
                if (NewArtwork.ArtistId == null || NewArtwork.ArtistId <= 0)
                {
                    ModelState.AddModelError("NewArtwork.ArtistId", "Please select an artist");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Handle image upload
                string imageUrl = "/images/artworks/default.svg";
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        imageUrl = await fileUploadService.UploadImageAsync(ImageFile, "artworks");
                    }
                    catch (ArgumentException ex)
                    {
                        ModelState.AddModelError("ImageFile", ex.Message);
                        await LoadAvailableArtistsAsync();
                        return Page();
                    }
                }

                // Direct SQL INSERT with parameterized query
                string insertQuery = @"
                    INSERT INTO Artworks (
                        Title, Description, ImageUrl, ArtworkType, Likes, 
                        CreatedDate, Price, IsForSale, Status, ArtistId
                    )
                    VALUES (
                        @Title, @Description, @ImageUrl, @ArtworkType, @Likes,
                        @CreatedDate, @Price, @IsForSale, @Status, @ArtistId
                    )";

                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@Title", SqlDbType.NVarChar, 150) { Value = NewArtwork.Title },
                    new SqlParameter("@Description", SqlDbType.NVarChar, 1000) { Value = NewArtwork.Description },
                    new SqlParameter("@ImageUrl", SqlDbType.NVarChar, 512) { Value = imageUrl },
                    new SqlParameter("@ArtworkType", SqlDbType.TinyInt) { Value = (int)NewArtwork.Type },
                    new SqlParameter("@Likes", SqlDbType.Int) { Value = 0 },
                    new SqlParameter("@CreatedDate", SqlDbType.DateTime2) { Value = DateTime.Now },
                    new SqlParameter("@Price", SqlDbType.Decimal) { Value = NewArtwork.IsForSale && NewArtwork.Price > 0 ? (object)NewArtwork.Price : DBNull.Value },
                    new SqlParameter("@IsForSale", SqlDbType.Bit) { Value = NewArtwork.IsForSale },
                    new SqlParameter("@Status", SqlDbType.TinyInt) { Value = 0 }, // Active
                    new SqlParameter("@ArtistId", SqlDbType.Int) { Value = NewArtwork.ArtistId }
                };

                // Execute INSERT command
                int rowsAffected = await dbHelper.ExecuteNonQueryAsync(insertQuery, insertParams);

                if (rowsAffected > 0)
                {
                    logger.LogInformation("Artwork added successfully: {Title} by ArtistId {ArtistId}", 
                        NewArtwork.Title, NewArtwork.ArtistId);
                    
                    TempData["SuccessMessage"] = $"Artwork '{NewArtwork.Title}' added successfully!";
                    return RedirectToPage("/Index");
                }
                else
                {
                    logger.LogWarning("Insert operation returned 0 rows affected");
                    ModelState.AddModelError("", "Failed to add artwork. Please try again.");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error adding artwork");
                
                // Handle specific SQL errors
                if (ex.Number == 547) // Foreign key violation
                {
                    ModelState.AddModelError("NewArtwork.ArtistId", "Selected artist does not exist");
                }
                else if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint violation
                {
                    ModelState.AddModelError("NewArtwork.Title", "An artwork with this title already exists");
                }
                else
                {
                    ModelState.AddModelError("", "Database error occurred. Please try again.");
                }

                await LoadAvailableArtistsAsync();
                return Page();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Database connection error");
                ModelState.AddModelError("", "Unable to connect to database. Please ensure SQL Server is running.");
                await LoadAvailableArtistsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error adding artwork");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                await LoadAvailableArtistsAsync();
                return Page();
            }
        }

        /// <summary>
        /// Load available artists for dropdown using direct SQL
        /// </summary>
        private async Task LoadAvailableArtistsAsync()
        {
            try
            {
                string query = @"
                    SELECT ArtistId, Name, Email, Status
                    FROM Artists
                    WHERE Status = 0
                    ORDER BY Name";

                DataTable dataTable = await dbHelper.ExecuteQueryAsync(query);

                AvailableArtists = new List<Artist>();
                foreach (DataRow row in dataTable.Rows)
                {
                    AvailableArtists.Add(new Artist
                    {
                        Id = Convert.ToInt32(row["ArtistId"]),
                        Name = row["Name"].ToString() ?? string.Empty,
                        Email = row["Email"].ToString() ?? string.Empty,
                        Status = ArtistStatus.Active
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading available artists");
                AvailableArtists = new List<Artist>();
            }
        }
    }
}

