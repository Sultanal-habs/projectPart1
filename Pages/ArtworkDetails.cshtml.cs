using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using projectPart1.Models;
using projectPart1.Data;
using System.Data;

namespace projectPart1.Pages
{
    public class ArtworkDetailsModel : PageModel
    {
        private readonly DatabaseHelper dbHelper;
        private readonly ILogger<ArtworkDetailsModel> logger;

        public ArtworkDetailsModel(DatabaseHelper dbHelper, ILogger<ArtworkDetailsModel> logger)
        {
            this.dbHelper = dbHelper;
            this.logger = logger;
        }

        public Artwork? Artwork { get; set; }
        public Artist? Artist { get; set; }
        public Exhibition? CurrentExhibition { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    ErrorMessage = "Invalid artwork ID";
                    return RedirectToPage("/Index");
                }

                // Direct SQL SELECT with JOIN to get complete artwork details (parameterized)
                string query = @"
                    SELECT 
                        aw.ArtworkId, aw.Title, aw.Description, aw.ImageUrl, 
                        aw.ArtworkType, aw.Likes, aw.CreatedDate, aw.Price, 
                        aw.IsForSale, aw.Status, aw.ArtistId,
                        a.Name AS ArtistName, a.Email AS ArtistEmail, 
                        a.Phone AS ArtistPhone, a.Bio AS ArtistBio,
                        a.ProfileImageUrl AS ArtistProfileImage,
                        e.ExhibitionId, e.Name AS ExhibitionName, 
                        e.Location AS ExhibitionLocation,
                        e.StartDate AS ExhibitionStartDate, 
                        e.EndDate AS ExhibitionEndDate
                    FROM Artworks aw
                    INNER JOIN Artists a ON aw.ArtistId = a.ArtistId
                    LEFT JOIN ExhibitionArtworks ea ON aw.ArtworkId = ea.ArtworkId
                    LEFT JOIN Exhibitions e ON ea.ExhibitionId = e.ExhibitionId 
                        AND e.Status = 1 AND GETDATE() BETWEEN e.StartDate AND e.EndDate
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

                // Map DataRow to objects
                DataRow row = dataTable.Rows[0];
                
                Artwork = new Artwork
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

                Artist = new Artist
                {
                    Id = Convert.ToInt32(row["ArtistId"]),
                    Name = row["ArtistName"].ToString() ?? string.Empty,
                    Email = row["ArtistEmail"].ToString() ?? string.Empty,
                    Phone = row["ArtistPhone"].ToString() ?? string.Empty,
                    Bio = row["ArtistBio"].ToString() ?? string.Empty,
                    ProfileImageUrl = row["ArtistProfileImage"].ToString() ?? "/images/artists/default.svg"
                };

                // Map exhibition if exists
                if (row["ExhibitionId"] != DBNull.Value)
                {
                    CurrentExhibition = new Exhibition
                    {
                        Id = Convert.ToInt32(row["ExhibitionId"]),
                        Name = row["ExhibitionName"].ToString() ?? string.Empty,
                        Location = row["ExhibitionLocation"].ToString() ?? string.Empty,
                        StartDate = Convert.ToDateTime(row["ExhibitionStartDate"]),
                        EndDate = Convert.ToDateTime(row["ExhibitionEndDate"])
                    };
                }

                return Page();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading artwork details: {ArtworkId}", id);
                ErrorMessage = "Error loading artwork details. Please try again.";
                return RedirectToPage("/Index");
            }
        }

        /// <summary>
        /// Handle like button - Direct SQL UPDATE
        /// </summary>
        public async Task<IActionResult> OnPostLikeAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    ErrorMessage = "Invalid artwork ID";
                    return RedirectToPage("/Index");
                }

                // Direct SQL UPDATE to increment likes (parameterized)
                string updateQuery = @"
                    UPDATE Artworks
                    SET Likes = Likes + 1
                    WHERE ArtworkId = @ArtworkId AND Status = 0";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ArtworkId", SqlDbType.Int) { Value = id }
                };

                int rowsAffected = await dbHelper.ExecuteNonQueryAsync(updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    logger.LogInformation("Artwork liked successfully: {ArtworkId}", id);
                    TempData["SuccessMessage"] = "Thank you for liking this artwork!";
                }
                else
                {
                    logger.LogWarning("Like failed - artwork not found or not active: {ArtworkId}", id);
                    TempData["ErrorMessage"] = "Unable to like this artwork.";
                }

                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error liking artwork: {ArtworkId}", id);
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToPage(new { id });
            }
        }

        /// <summary>
        /// Handle delete button - Direct SQL DELETE
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    ErrorMessage = "Invalid artwork ID";
                    return RedirectToPage("/Index");
                }

                // Get artwork title for confirmation message
                string titleQuery = "SELECT Title FROM Artworks WHERE ArtworkId = @ArtworkId";
                SqlParameter[] titleParams = new SqlParameter[]
                {
                    new SqlParameter("@ArtworkId", SqlDbType.Int) { Value = id }
                };

                object? titleResult = await dbHelper.ExecuteScalarAsync(titleQuery, titleParams);
                string artworkTitle = titleResult?.ToString() ?? "Unknown";

                // Direct SQL DELETE with parameterized query
                string deleteQuery = @"
                    DELETE FROM Artworks
                    WHERE ArtworkId = @ArtworkId";

                SqlParameter[] deleteParams = new SqlParameter[]
                {
                    new SqlParameter("@ArtworkId", SqlDbType.Int) { Value = id }
                };

                int rowsAffected = await dbHelper.ExecuteNonQueryAsync(deleteQuery, deleteParams);

                if (rowsAffected > 0)
                {
                    logger.LogInformation("Artwork deleted successfully: {ArtworkId} - {Title}", id, artworkTitle);
                    TempData["SuccessMessage"] = $"Artwork '{artworkTitle}' deleted successfully!";
                    return RedirectToPage("/Index");
                }
                else
                {
                    logger.LogWarning("Delete failed - artwork not found: {ArtworkId}", id);
                    TempData["ErrorMessage"] = "Artwork not found or already deleted.";
                    return RedirectToPage("/Index");
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error deleting artwork: {ArtworkId}", id);

                // Handle foreign key constraint violations
                if (ex.Number == 547)
                {
                    TempData["ErrorMessage"] = "Cannot delete this artwork because it is referenced in other records.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Database error occurred while deleting artwork.";
                }

                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting artwork: {ArtworkId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the artwork.";
                return RedirectToPage(new { id });
            }
        }
    }
}
