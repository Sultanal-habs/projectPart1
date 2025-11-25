using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using projectPart1.Models;
using projectPart1.Data;
using System.Data;

namespace projectPart1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseHelper dbHelper;
        private readonly ILogger<IndexModel> logger;

        public IndexModel(DatabaseHelper dbHelper, ILogger<IndexModel> logger)
        {
            this.dbHelper = dbHelper;
            this.logger = logger;
        }

        public List<Artwork> Artworks { get; set; } = new List<Artwork>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public ArtworkType? FilterType { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Build dynamic SQL query based on filters
                string query = @"
                    SELECT 
                        aw.ArtworkId, aw.Title, aw.Description, aw.ImageUrl, 
                        aw.ArtworkType, aw.Likes, aw.CreatedDate, aw.Price, 
                        aw.IsForSale, aw.Status, aw.ArtistId,
                        a.Name AS ArtistName, a.Email AS ArtistEmail, a.Phone AS ArtistPhone
                    FROM Artworks aw
                    INNER JOIN Artists a ON aw.ArtistId = a.ArtistId
                    WHERE aw.Status = 0"; // 0 = Active

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Add search filter
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query += " AND (aw.Title LIKE @SearchTerm OR aw.Description LIKE @SearchTerm OR a.Name LIKE @SearchTerm)";
                    parameters.Add(new SqlParameter("@SearchTerm", $"%{SearchTerm}%"));
                    logger.LogInformation("Searching artworks with term: {SearchTerm}", SearchTerm);
                }

                // Add type filter
                if (FilterType.HasValue)
                {
                    query += " AND aw.ArtworkType = @ArtworkType";
                    parameters.Add(new SqlParameter("@ArtworkType", (int)FilterType.Value));
                    logger.LogInformation("Filtering artworks by type: {FilterType}", FilterType);
                }

                query += " ORDER BY aw.CreatedDate DESC";

                // Execute parameterized query
                DataTable dataTable = await dbHelper.ExecuteQueryAsync(query, parameters.ToArray());

                // Map DataTable to Artwork objects
                Artworks = MapDataTableToArtworks(dataTable);
                
                logger.LogInformation("Retrieved {Count} active artworks", Artworks.Count);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Database connection error");
                ErrorMessage = "Unable to connect to database. Please ensure SQL Server is running.";
                Artworks = new List<Artwork>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading artworks");
                ErrorMessage = "An error occurred while loading artworks. Please try again.";
                Artworks = new List<Artwork>();
            }
        }

        /// <summary>
        /// Map DataTable rows to Artwork objects
        /// </summary>
        private List<Artwork> MapDataTableToArtworks(DataTable dataTable)
        {
            List<Artwork> artworks = new List<Artwork>();

            foreach (DataRow row in dataTable.Rows)
            {
                Artwork artwork = new Artwork
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
                    ArtistName = row["ArtistName"].ToString() ?? string.Empty,
                    ArtistEmail = row["ArtistEmail"].ToString() ?? string.Empty,
                    ArtistPhone = row["ArtistPhone"].ToString() ?? string.Empty
                };

                artworks.Add(artwork);
            }

            return artworks;
        }
    }
}

