using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using projectPart1.Models;
using projectPart1.Data;
using System.Data;

namespace projectPart1.Pages
{
    public class ArtistsModel : PageModel
    {
        private readonly DatabaseHelper dbHelper;
        private readonly ILogger<ArtistsModel> logger;

        public ArtistsModel(DatabaseHelper dbHelper, ILogger<ArtistsModel> logger)
        {
            this.dbHelper = dbHelper;
            this.logger = logger;
        }

        public List<Artist> Artists { get; set; } = new List<Artist>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Direct SQL SELECT with parameterized query using database view
                string query = @"
                    SELECT 
                        ArtistId, ArtistName, Email, Phone, Bio, ProfileImageUrl,
                        StatusName, JoinedDate, TotalArtworks, TotalLikes,
                        AverageLikesPerArtwork, ExhibitionsParticipated
                    FROM vw_ArtistPortfolio
                    WHERE StatusName = 'Active'";

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Add search filter with parameterized query
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query += " AND (ArtistName LIKE @SearchTerm OR Email LIKE @SearchTerm OR Bio LIKE @SearchTerm)";
                    parameters.Add(new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 256) 
                    { 
                        Value = $"%{SearchTerm}%" 
                    });
                    logger.LogInformation("Searching artists with term: {SearchTerm}", SearchTerm);
                }

                query += " ORDER BY TotalLikes DESC, JoinedDate DESC";

                // Execute parameterized SELECT query
                DataTable dataTable = await dbHelper.ExecuteQueryAsync(query, parameters.ToArray());

                // Map DataTable to Artist objects
                Artists = MapDataTableToArtists(dataTable);

                logger.LogInformation("Retrieved {Count} active artists from database", Artists.Count);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Database connection error loading artists");
                ErrorMessage = "Unable to connect to database. Please ensure SQL Server is running.";
                Artists = new List<Artist>();
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error loading artists");
                ErrorMessage = "Database error occurred. Please try again.";
                Artists = new List<Artist>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading artists");
                ErrorMessage = "An error occurred while loading artists. Please try again.";
                Artists = new List<Artist>();
            }
        }

        /// <summary>
        /// Map DataTable rows to Artist objects with proper null handling
        /// </summary>
        private List<Artist> MapDataTableToArtists(DataTable dataTable)
        {
            List<Artist> artists = new List<Artist>();

            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    Artist artist = new Artist
                    {
                        Id = Convert.ToInt32(row["ArtistId"]),
                        Name = row["ArtistName"]?.ToString() ?? string.Empty,
                        Email = row["Email"]?.ToString() ?? string.Empty,
                        Phone = row["Phone"]?.ToString() ?? string.Empty,
                        Bio = row["Bio"]?.ToString() ?? string.Empty,
                        ProfileImageUrl = row["ProfileImageUrl"]?.ToString() ?? "/images/artists/default.svg",
                        JoinedDate = row["JoinedDate"] != DBNull.Value 
                            ? Convert.ToDateTime(row["JoinedDate"]) 
                            : DateTime.Now,
                        Status = row["StatusName"]?.ToString() == "Active" ? ArtistStatus.Active : ArtistStatus.Inactive
                    };

                    artists.Add(artist);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error mapping artist row, skipping");
                }
            }

            return artists;
        }
    }
}
