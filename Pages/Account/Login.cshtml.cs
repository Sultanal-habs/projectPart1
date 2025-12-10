using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;
namespace projectPart1.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<LoginModel> _logger;
        public LoginModel(ArtGalleryDbContext context, ILogger<LoginModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        [BindProperty]
        public string Username { get; set; } = string.Empty;
        [BindProperty]
        public string Password { get; set; } = string.Empty;
        [BindProperty]
        public bool RememberMe { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ReturnUrl { get; set; }
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/Index";
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                Response.Redirect(ReturnUrl);
            }
        }
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/Index";
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Username and password are required";
                    return Page();
                }
                var user = await _context.Users
                    .Where(u => u.Username == Username && u.IsActive)
                    .FirstOrDefaultAsync();
                if (user == null)
                {
                    _logger.LogWarning("Login attempt for non-existent user: {Username}", Username);
                    ErrorMessage = "Invalid username or password";
                    return Page();
                }
                if (!user.VerifyPassword(Password))
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", Username);
                    ErrorMessage = "Invalid username or password";
                    return Page();
                }
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Email", user.Email);
                _logger.LogInformation("User logged in successfully: {Username}", user.Username);
                return Redirect(ReturnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }
}