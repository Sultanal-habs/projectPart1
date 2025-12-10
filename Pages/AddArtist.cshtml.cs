using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Models;
using projectPart1.Services;
using projectPart1.Data;

namespace projectPart1.Pages
{
    public class AddArtistModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly FileUploadService _fileUploadService;
        private readonly ILogger<AddArtistModel> _logger;

        public AddArtistModel(ArtGalleryDbContext context, FileUploadService fileUploadService, ILogger<AddArtistModel> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        [BindProperty]
        public Artist NewArtist { get; set; } = new Artist();

        [BindProperty]
        public IFormFile? ProfileImageFile { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Check if email already exists
                bool emailExists = await _context.Artists.AnyAsync(a => a.Email == NewArtist.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("NewArtist.Email", "Email already exists");
                    return Page();
                }

                // Handle image upload
                if (ProfileImageFile != null && ProfileImageFile.Length > 0)
                {
                    string imagePath = await _fileUploadService.UploadImageAsync(ProfileImageFile, "artists");
                    NewArtist.ProfileImageUrl = imagePath;
                }
                else
                {
                    NewArtist.ProfileImageUrl = "/images/artists/default.jpg";
                }

                // Set defaults
                NewArtist.JoinedDate = DateTime.Now;
                NewArtist.Status = ArtistStatus.Active;

                // Get current user from session
                var username = HttpContext.Session.GetString("Username");
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                    if (user != null)
                    {
                        NewArtist.UserId = user.UserId;
                    }
                }

                // Add to database
                _context.Artists.Add(NewArtist);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Artist {Name} added successfully", NewArtist.Name);
                TempData["SuccessMessage"] = $"Artist profile for {NewArtist.Name} created successfully!";
                
                return RedirectToPage("/Artists");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding artist");
                ModelState.AddModelError("", "A database error occurred. Please check your input and try again.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding artist");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return Page();
            }
        }
    }
}
