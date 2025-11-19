using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ArtworkService artworkService;
        private readonly ILogger<IndexModel> logger;

        public IndexModel(ArtworkService artworkService,ILogger<IndexModel> logger)
        {
            this.artworkService=artworkService;
            this.logger=logger;
        }

        public List<Artwork> Artworks{get;set;}=new List<Artwork>();

        [BindProperty(SupportsGet=true)]
        public string? SearchTerm {get;set;}

        [BindProperty(SupportsGet =true)]
        public ArtworkType? FilterType{get;set;}

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage{get;set;}

        public void OnGet()
        {
            try
            {
                bool hasSearch=!string.IsNullOrWhiteSpace(SearchTerm);
                if(hasSearch)
                {
                    Artworks =artworkService.SearchArtworks(SearchTerm);
                    logger.LogInformation("Searched artworks with term: {SearchTerm}",SearchTerm);
                }
                else if(FilterType.HasValue)
                {
                    Artworks=artworkService.FilterByType(FilterType);
                    logger.LogInformation("Filtered artworks by type: {FilterType}",FilterType);
                }
                else
                {
                    Artworks=artworkService.GetActiveArtworks();
                    logger.LogInformation("Retrieved all active artworks");
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"Error loading artworks");
                ErrorMessage ="An error occurred while loading artworks. Please try again.";
                Artworks=new List<Artwork>();
            }
        }
    }
}

