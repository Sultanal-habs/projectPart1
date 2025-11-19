using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class ArtistsModel : PageModel
    {
        private readonly ArtistService artistService;
        private readonly ILogger<ArtistsModel> logger;

        public ArtistsModel(ArtistService artistService, ILogger<ArtistsModel> logger)
        {
            this.artistService = artistService;
            this.logger = logger;
        }

        public List<Artist> Artists { get; set; } = new List<Artist>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            try
            {
                bool hasSearch = !string.IsNullOrWhiteSpace(SearchTerm);
                if (hasSearch)
                {
                    Artists = artistService.SearchArtists(SearchTerm);
                    logger.LogInformation("Searched artists with term: {SearchTerm}", SearchTerm);
                }
                else
                {
                    Artists = artistService.GetActiveArtists();
                    logger.LogInformation("Retrieved all active artists");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading artists");
                ErrorMessage = "An error occurred while loading artists. Please try again.";
                Artists = new List<Artist>();
            }
        }
    }
}
