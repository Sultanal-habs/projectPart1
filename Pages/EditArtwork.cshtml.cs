using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using projectPart1.Models;
using projectPart1.Data;
using projectPart1.Services;
using System.Data;

namespace projectPart1.Pages
{
    public class EditArtworkModel : PageModel
    {
        private readonly DatabaseHelper dbHelper;
        private readonly FileUploadService fileUploadService;
        private readonly ILogger<EditArtworkModel> logger;

        public EditArtworkModel(DatabaseHelper dbHelper, FileUploadService fileUploadService, ILogger<EditArtworkModel> logger)
        {
            this.dbHelper = dbHelper;
            this.fileUploadService = fileUploadService;
            this.logger = logger;
        }

        [BindProperty]
        public Artwork EditArtwork { get; set; } = new Artwork();

        [BindProperty]
        public IFormFile? NewImageFile { get; set; }

        public string? CurrentImageUrl { get; set; }
        public List<Artist> AvailableArtists { get; set; } = new List<Artist>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    ErrorMessage = "Invalid artwork ID";
                    return RedirectToPage("/Index");
                }

                // Direct SQL SELECT to get artwork by ID (parameterized)
                string query = @"
                    SELECT 
                        aw.ArtworkId, aw.Title, aw.Description, aw.ImageUrl, 
                        aw.ArtworkType, aw.Likes, aw.CreatedDate, aw.Price, 
                        aw.IsForSale, aw.Status, aw.ArtistId,
                        a.Name AS ArtistName
                    FROM Artworks aw
                    INNER JOIN Artists a ON aw.ArtistId = a.ArtistId
                    WHERE aw.ArtworkId = @ArtworkId";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ArtworkId", SqlDbType.Int) { Value = id }
                };

                DataTable dataTable = await dbHelper.ExecuteQueryAsync(query, parameters);

                if (dataTable.Rows.Count == 0)
                {
                    ErrorMessage = "Artwork not found";
                    return RedirectToPage("/Index");
                }

                // Map to Artwork object
                DataRow row = dataTable.Rows[0];
                EditArtwork = new Artwork
                {
                    Id = Convert.ToInt32(row["ArtworkId"]),
                    Title = row["Title"].ToString() ?? string.Empty,
                    Description = row["Description"].ToString() ?? string.Empty,
                    ImageUrl = row["ImageUrl"].ToString() ?? string.Empty,
                    Type = (ArtworkType)Convert.ToInt32(row["ArtworkType"]),
                    Likes = Convert.ToInt32(row["Likes"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0,
                    IsForSale = Convert.ToBoolean(row["IsForSale"]),
                    Status = (ArtworkStatus)Convert.ToInt32(row["Status"]),
                    ArtistId = Convert.ToInt32(row["ArtistId"]),
                    ArtistName = row["ArtistName"].ToString() ?? string.Empty
                };

                CurrentImageUrl = EditArtwork.ImageUrl;

                // Load available artists
                await LoadAvailableArtistsAsync();

                return Page();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading artwork for edit: {ArtworkId}", id);
                ErrorMessage = "Error loading artwork. Please try again.";
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Model state is invalid for artwork update");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Additional validation
                if (EditArtwork.Price < 0)
                {
                    ModelState.AddModelError("EditArtwork.Price", "Price cannot be negative");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                if (EditArtwork.Price > 999999)
                {
                    ModelState.AddModelError("EditArtwork.Price", "Price exceeds maximum allowed value");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Handle new image upload
                string imageUrl = EditArtwork.ImageUrl;
                if (NewImageFile != null && NewImageFile.Length > 0)
                {
                    try
                    {
                        // Delete old image if not default
                        if (!string.IsNullOrEmpty(CurrentImageUrl) && !CurrentImageUrl.Contains("default"))
                        {
                            fileUploadService.DeleteImage(CurrentImageUrl);
                        }

                        // Upload new image
                        imageUrl = await fileUploadService.UploadImageAsync(NewImageFile, "artworks");
                    }
                    catch (ArgumentException ex)
                    {
                        ModelState.AddModelError("NewImageFile", ex.Message);
                        await LoadAvailableArtistsAsync();
                        return Page();
                    }
                }

                // Direct SQL UPDATE with parameterized query
                string updateQuery = @"
                    UPDATE Artworks
                    SET 
                        Title = @Title,
                        Description = @Description,
                        ImageUrl = @ImageUrl,
                        ArtworkType = @ArtworkType,
                        Price = @Price,
                        IsForSale = @IsForSale,
                        ArtistId = @ArtistId
                    WHERE ArtworkId = @ArtworkId";

                SqlParameter[] updateParams = new SqlParameter[]
                {
                    new SqlParameter("@ArtworkId", SqlDbType.Int) { Value = EditArtwork.Id },
                    new SqlParameter("@Title", SqlDbType.NVarChar, 150) { Value = EditArtwork.Title },
                    new SqlParameter("@Description", SqlDbType.NVarChar, 1000) { Value = EditArtwork.Description },
                    new SqlParameter("@ImageUrl", SqlDbType.NVarChar, 512) { Value = imageUrl },
                    new SqlParameter("@ArtworkType", SqlDbType.TinyInt) { Value = (int)EditArtwork.Type },
                    new SqlParameter("@Price", SqlDbType.Decimal) 
                    { 
                        Value = EditArtwork.IsForSale && EditArtwork.Price > 0 ? (object)EditArtwork.Price : DBNull.Value 
                    },
                    new SqlParameter("@IsForSale", SqlDbType.Bit) { Value = EditArtwork.IsForSale },
                    new SqlParameter("@ArtistId", SqlDbType.Int) { Value = EditArtwork.ArtistId }
                };

                // Execute UPDATE command
                int rowsAffected = await dbHelper.ExecuteNonQueryAsync(updateQuery, updateParams);

                if (rowsAffected > 0)
                {
                    logger.LogInformation("Artwork updated successfully: {ArtworkId} - {Title}", 
                        EditArtwork.Id, EditArtwork.Title);
                    
                    TempData["SuccessMessage"] = $"Artwork '{EditArtwork.Title}' updated successfully!";
                    return RedirectToPage("/ArtworkDetails", new { id = EditArtwork.Id });
                }
                else
                {
                    logger.LogWarning("Update operation affected 0 rows for ArtworkId: {ArtworkId}", EditArtwork.Id);
                    ModelState.AddModelError("", "Artwork not found or no changes were made.");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error updating artwork: {ArtworkId}", EditArtwork.Id);

                // Handle specific SQL errors
                if (ex.Number == 547) // Foreign key violation
                {
                    ModelState.AddModelError("EditArtwork.ArtistId", "Selected artist does not exist");
                }
                else if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint
                {
                    ModelState.AddModelError("EditArtwork.Title", "An artwork with this title already exists");
                }
                else
                {
                    ModelState.AddModelError("", $"Database error: {ex.Message}");
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
                logger.LogError(ex, "Unexpected error updating artwork");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                await LoadAvailableArtistsAsync();
                return Page();
            }
        }

        [TempData]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Load available artists using direct SQL SELECT
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
