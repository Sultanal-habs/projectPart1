using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace projectPart1.Filters
{
    public class AuthenticationFilter : IPageFilter
    {
        private readonly ILogger<AuthenticationFilter> _logger;
        private static readonly string[] PublicPages = 
        {
            "/Account/Login",
            "/Account/Register",
            "/Error"
        };
        public AuthenticationFilter(ILogger<AuthenticationFilter> logger)
        {
            _logger = logger;
        }
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var path = httpContext.Request.Path.Value ?? string.Empty;
            bool isPublicPage = PublicPages.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
            bool isStaticFile = path.Contains(".");
            if (isPublicPage || isStaticFile)
            {
                return;
            }
            var username = httpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Unauthorized access attempt to: {Path}", path);
                var returnUrl = System.Net.WebUtility.UrlEncode(path);
                context.Result = new RedirectResult($"/Account/Login?returnUrl={returnUrl}");
            }
        }
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }
    }
}