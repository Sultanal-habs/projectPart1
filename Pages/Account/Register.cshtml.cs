using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;
using Microsoft.Extensions.Logging;
namespace projectPart1.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        public RegisterModel(ArtGalleryDbContext context, ILogger<RegisterModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        [BindProperty]
        public User NewUser { get; set; } = new User();
        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public void OnGet()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                Response.Redirect("/Index");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ErrorMessage = "Please correct the errors and try again";
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(NewUser.PasswordHash) || NewUser.PasswordHash != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match";
                    return Page();
                }
                var existingUser = await (from u in _context.Users
                                          where u.Username == NewUser.Username
                                          select u).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    ErrorMessage = "Username already exists";
                    return Page();
                }
                var existingEmail = await _context.Users
                    .AnyAsync(u => u.Email == NewUser.Email);
                if (existingEmail)
                {
                    ErrorMessage = "Email already registered";
                    return Page();
                }
                string rawPassword = NewUser.PasswordHash;
                NewUser.PasswordHash = Models.User.HashPassword(rawPassword);
                NewUser.CreatedDate = DateTime.Now;
                NewUser.IsActive = true;
                if (string.IsNullOrWhiteSpace(NewUser.UserRole))
                {
                    NewUser.UserRole = "User";
                }
                _context.Users.Add(NewUser);
                await _context.SaveChangesAsync();
                _logger.LogInformation("New user registered: {Username}", NewUser.Username);
                HttpContext.Session.SetString("Username", NewUser.Username);
                HttpContext.Session.SetString("FullName", NewUser.FullName);
                HttpContext.Session.SetString("UserRole", NewUser.UserRole);
                HttpContext.Session.SetInt32("UserId", NewUser.UserId);
                HttpContext.Session.SetString("Email", NewUser.Email);
                TempData["SuccessMessage"] = "Registration successful! Welcome to Art Gallery.";
                return RedirectToPage("/Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during registration");
                ErrorMessage = "An error occurred during registration. Please try again.";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ErrorMessage = "An unexpected error occurred. Please try again.";
                return Page();
            }
        }
    }
}
