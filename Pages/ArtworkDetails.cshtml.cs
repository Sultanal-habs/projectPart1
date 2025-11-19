using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class ArtworkDetailsModel : PageModel
    {
        private readonly ArtworkService artworkService;

        public ArtworkDetailsModel(ArtworkService artworkService)
        {
            this.artworkService=artworkService;
        }

        public Artwork? Artwork{get;set;}

        public IActionResult OnGet(int id)
        {
            Artwork =artworkService.GetArtworkById(id);
            if(Artwork==null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPostLike(int id)
        {
            artworkService.IncrementLikes(id);
            return RedirectToPage(new{id});
        }
    }
}
