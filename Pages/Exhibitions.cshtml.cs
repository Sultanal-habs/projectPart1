using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class ExhibitionsModel : PageModel
    {
        private readonly ExhibitionService exhibitionService;
        private readonly ILogger<ExhibitionsModel> logger;

        public ExhibitionsModel(ExhibitionService exhibitionService, ILogger<ExhibitionsModel> logger)
        {
            this.exhibitionService = exhibitionService;
            this.logger = logger;
        }

        public List<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();
        public List<Exhibition> ActiveExhibitions { get; set; } = new List<Exhibition>();
        public List<Exhibition> UpcomingExhibitions { get; set; } = new List<Exhibition>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public ExhibitionStatus? FilterStatus { get; set; }

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
                    Exhibitions = exhibitionService.SearchExhibitions(SearchTerm);
                    logger.LogInformation("Searched exhibitions with term: {SearchTerm}", SearchTerm);
                }
                else if (FilterStatus.HasValue)
                {
                    Exhibitions = exhibitionService.FilterByStatus(FilterStatus.Value);
                    logger.LogInformation("Filtered exhibitions by status: {FilterStatus}", FilterStatus);
                }
                else
                {
                    Exhibitions = exhibitionService.GetAllExhibitions();
                    logger.LogInformation("Retrieved all exhibitions");
                }

                ActiveExhibitions = exhibitionService.GetActiveExhibitions();
                UpcomingExhibitions = exhibitionService.GetUpcomingExhibitions();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading exhibitions");
                ErrorMessage = "An error occurred while loading exhibitions. Please try again.";
                Exhibitions = new List<Exhibition>();
                ActiveExhibitions = new List<Exhibition>();
                UpcomingExhibitions = new List<Exhibition>();
            }
        }
    }
}
