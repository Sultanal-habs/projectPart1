using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
namespace projectPart1.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDetails { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        private readonly ILogger<ErrorModel> _logger;
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            try
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                ErrorMessage = "An unexpected error occurred";
                ErrorDetails = "Please try again or contact support if the problem persists";
                _logger.LogError("Error page accessed - Request ID: {RequestId}", RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading error page");
            }
        }
    }
}