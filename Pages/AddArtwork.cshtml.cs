using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Models;
using projectPart1.Data;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class AddArtworkModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly FileUploadService _fileUploadService;
        private readonly ILogger<AddArtworkModel> _logger;

        public AddArtworkModel(ArtGalleryDbContext context, FileUploadService fileUploadService, ILogger<AddArtworkModel> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        [BindProperty]
        public Artwork NewArtwork { get; set; } = new Artwork();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public List<Artist> AvailableArtists { get; set; } = new List<Artist>();

        public async Task OnGetAsync()
        {
            await LoadAvailableArtistsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Remove validation for properties that are not in the form
                ModelState.Remove("NewArtwork.ImageUrl");
                ModelState.Remove("NewArtwork.ArtistName");
                ModelState.Remove("NewArtwork.ArtistEmail");
                ModelState.Remove("NewArtwork.ArtistPhone");
                
                if (!ModelState.IsValid)
                {
                    // Log all validation errors for debugging
                    foreach (var error in ModelState)
                    {
                        foreach (var subError in error.Value.Errors)
                        {
                            _logger.LogWarning("Validation error for {Key}: {Error}", error.Key, subError.ErrorMessage);
                        }
                    }
                    
                    _logger.LogWarning("Model state is invalid for new artwork");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Check if artist exists
                bool artistExists = await _context.Artists.AnyAsync(a => a.Id == NewArtwork.ArtistId);
                if (!artistExists)
                {
                    ModelState.AddModelError("NewArtwork.ArtistId", "Selected artist does not exist");
                    await LoadAvailableArtistsAsync();
                    return Page();
                }

                // Handle image upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        string imagePath = await _fileUploadService.UploadImageAsync(ImageFile, "artworks");
                        NewArtwork.ImageUrl = imagePath;
                    }
                    catch (ArgumentException ex)
                    {
                        ModelState.AddModelError("ImageFile", ex.Message);
                        await LoadAvailableArtistsAsync();
                        return Page();
                    }
                }
                else
                {
                    NewArtwork.ImageUrl = "/images/artworks/default.svg";
                }

                // Set defaults
                NewArtwork.CreatedDate = DateTime.Now;
                NewArtwork.Likes = 0;
                NewArtwork.Status = ArtworkStatus.Active;

                // Add to database using Entity Framework
                _context.Artworks.Add(NewArtwork);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Artwork added successfully: {Title} by ArtistId {ArtistId}", 
                    NewArtwork.Title, NewArtwork.ArtistId);
                TempData["SuccessMessage"] = $"Artwork '{NewArtwork.Title}' added successfully!";
                
                return RedirectToPage("/Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding artwork");
                
                if (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    ModelState.AddModelError("NewArtwork.ArtistId", "Selected artist does not exist");
                }
                else if (ex.InnerException?.Message.Contains("duplicate") == true || 
                         ex.InnerException?.Message.Contains("unique") == true)
                {
                    ModelState.AddModelError("NewArtwork.Title", "An artwork with this title already exists");
                }
                else
                {
                    ModelState.AddModelError("", "A database error occurred. Please check your input and try again.");
                }
                
                await LoadAvailableArtistsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding artwork");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                await LoadAvailableArtistsAsync();
                return Page();
            }
        }

        private async Task LoadAvailableArtistsAsync()
        {
            try
            {
                AvailableArtists = await _context.Artists
                    .Where(a => a.Status == ArtistStatus.Active)
                    .OrderBy(a => a.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading available artists");
                AvailableArtists = new List<Artist>();
            }
        }
    }
}