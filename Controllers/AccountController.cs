using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;

namespace projectPart1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ArtGalleryDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return Redirect(returnUrl ?? "/User");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Username and password are required";
                return View();
            }

            try
            {
                var user = await _context.Users
                    .Where(u => u.Username == username && u.IsActive)
                    .FirstOrDefaultAsync();

                if (user == null || !user.VerifyPassword(password))
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", username);
                    ViewBag.ErrorMessage = "Invalid username or password";
                    return View();
                }

                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Email", user.Email);

                _logger.LogInformation("User logged in successfully: {Username}", user.Username);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (user.UserRole == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                
                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return View(user);
            }

            ModelState.Remove("PasswordHash");
            ModelState.Remove("UserRole");

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    {
                        ModelState.AddModelError("Username", "Username already exists");
                        return View(user);
                    }

                    if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    {
                        ModelState.AddModelError("Email", "Email already registered");
                        return View(user);
                    }

                    user.PasswordHash = projectPart1.Models.User.HashPassword(password);
                    user.UserRole = "User";
                    user.IsActive = true;
                    user.CreatedDate = DateTime.Now;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Registration successful! Please login.";
                    return RedirectToAction(nameof(Login));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration");
                    ModelState.AddModelError("", "An error occurred during registration.");
                }
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
