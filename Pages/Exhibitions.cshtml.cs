using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;
namespace projectPart1.Pages
{
    public class ExhibitionsModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<ExhibitionsModel> _logger;
        public ExhibitionsModel(ArtGalleryDbContext context, ILogger<ExhibitionsModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();
        public List<Exhibition> ActiveExhibitions { get; set; } = new List<Exhibition>();
        public List<Exhibition> UpcomingExhibitions { get; set; } = new List<Exhibition>();
        public Dictionary<ExhibitionStatus, int> ExhibitionsByStatus { get; set; } = new Dictionary<ExhibitionStatus, int>();
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public ExhibitionStatus? FilterStatus { get; set; }
        [TempData]
        public string? SuccessMessage { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                var baseQuery = from e in _context.Exhibitions
                                where e.Status != ExhibitionStatus.Cancelled
                                orderby e.StartDate descending
                                select e;
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    Exhibitions = await baseQuery
                        .Where(e => e.Name.Contains(SearchTerm) || 
                                    e.Description.Contains(SearchTerm) || 
                                    e.Location.Contains(SearchTerm))
                        .ToListAsync();
                    _logger.LogInformation("Searched exhibitions with term: {SearchTerm}, found {Count}", SearchTerm, Exhibitions.Count);
                }
                else if (FilterStatus.HasValue)
                {
                    Exhibitions = await baseQuery
                        .Where(e => e.Status == FilterStatus.Value)
                        .ToListAsync();
                    _logger.LogInformation("Filtered exhibitions by status: {Status}", FilterStatus);
                }
                else
                {
                    Exhibitions = await baseQuery.ToListAsync();
                }
                ActiveExhibitions = await _context.Exhibitions
                    .Where(e => e.Status == ExhibitionStatus.Active && 
                                DateTime.Now >= e.StartDate && 
                                DateTime.Now <= e.EndDate)
                    .OrderBy(e => e.StartDate)
                    .Take(5)
                    .ToListAsync();
                UpcomingExhibitions = await (from e in _context.Exhibitions
                                             where e.Status == ExhibitionStatus.Upcoming && 
                                                   e.StartDate > DateTime.Now
                                             orderby e.StartDate
                                             select e)
                                            .Take(5)
                                            .ToListAsync();
                ExhibitionsByStatus = await _context.Exhibitions
                    .GroupBy(e => e.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);
                _logger.LogInformation("Retrieved {Total} exhibitions, {Active} active, {Upcoming} upcoming", 
                    Exhibitions.Count, ActiveExhibitions.Count, UpcomingExhibitions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading exhibitions");
                ErrorMessage = "An error occurred while loading exhibitions. Please try again.";
                Exhibitions = new List<Exhibition>();
                ActiveExhibitions = new List<Exhibition>();
                UpcomingExhibitions = new List<Exhibition>();
            }
        }
    }
}