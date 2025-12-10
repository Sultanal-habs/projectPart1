using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Models;
using projectPart1.Services;
using projectPart1.Data;

namespace projectPart1.Pages
{
    public class AddExhibitionModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly FileUploadService _fileUploadService;
        private readonly ILogger<AddExhibitionModel> _logger;

        public AddExhibitionModel(ArtGalleryDbContext context, FileUploadService fileUploadService, ILogger<AddExhibitionModel> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        [BindProperty]
        public Exhibition NewExhibition { get; set; } = new Exhibition();

        [BindProperty]
        public IFormFile? BannerImageFile { get; set; }

        public void OnGet()
        {
            DateTime today = DateTime.Today;
            NewExhibition.StartDate = today.AddDays(7);
            NewExhibition.EndDate = today.AddDays(37);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid");
                    return Page();
                }

                DateTime today = DateTime.Today;
                if (NewExhibition.StartDate < today)
                {
                    ModelState.AddModelError("NewExhibition.StartDate", "Start date must be in the future");
                    return Page();
                }

                if (NewExhibition.EndDate <= NewExhibition.StartDate)
                {
                    ModelState.AddModelError("NewExhibition.EndDate", "End date must be after start date");
                    return Page();
                }

                TimeSpan diff = NewExhibition.EndDate - NewExhibition.StartDate;
                int days = diff.Days;
                
                if (days < 1)
                {
                    ModelState.AddModelError("", "Exhibition must be at least one day");
                    return Page();
                }

                if (days > 365)
                {
                    ModelState.AddModelError("", "Exhibition cannot be longer than one year");
                    return Page();
                }

                // Handle image upload
                if (BannerImageFile != null && BannerImageFile.Length > 0)
                {
                    string imagePath = await _fileUploadService.UploadImageAsync(BannerImageFile, "exhibitions");
                    NewExhibition.BannerImageUrl = imagePath;
                }
                else
                {
                    NewExhibition.BannerImageUrl = "/images/exhibitions/default.jpg";
                }

                // Set defaults
                NewExhibition.CreatedDate = DateTime.Now;
                NewExhibition.Status = ExhibitionStatus.Upcoming;

                // Add to database
                _context.Exhibitions.Add(NewExhibition);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Exhibition added: {Name}", NewExhibition.Name);
                TempData["SuccessMessage"] = $"Exhibition '{NewExhibition.Name}' created successfully!";
                
                return RedirectToPage("/Exhibitions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding exhibition");
                ModelState.AddModelError("", "An error occurred. Please try again.");
                return Page();
            }
        }
    }
}