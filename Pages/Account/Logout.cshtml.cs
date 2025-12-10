using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;

namespace projectPart1.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ArtGalleryDbContext context, ILogger<LogoutModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");

                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _context.Users
                        .Where(u => u.Username == username)
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        user.LastLoginDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User logged out: {Username}", username);
                    }
                }

                // Clear session
                HttpContext.Session.Clear();

                TempData["SuccessMessage"] = "You have been logged out successfully.";

                // Redirect to login page
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                HttpContext.Session.Clear();
                return RedirectToPage("/Account/Login");
            }
        }
    }
}