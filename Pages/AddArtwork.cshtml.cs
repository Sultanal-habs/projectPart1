using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class AddArtworkModel : PageModel
    {
        private readonly ArtworkService artworkService;
        private readonly FileUploadService fileUploadService;
        private readonly ILogger<AddArtworkModel> logger;

        public AddArtworkModel(ArtworkService artworkService,FileUploadService fileUploadService,ILogger<AddArtworkModel> logger)
        {
            this.artworkService=artworkService;
            this.fileUploadService=fileUploadService;
            this.logger =logger;
        }

        [BindProperty]
        public Artwork NewArtwork{get;set;}=new Artwork();

        [BindProperty]
        public IFormFile ImageFile{get;set;}

        public void OnGet(){}

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                bool isValid=ModelState.IsValid;
                if(!isValid)
                {
                    logger.LogWarning("Model state is invalid");
                    return Page();
                }

                decimal price=NewArtwork.Price;
                if(price<0)
                {
                    ModelState.AddModelError("NewArtwork.Price","Price cannot be negative");
                    return Page();
                }

                if(price>999999)
                {
                    ModelState.AddModelError("NewArtwork.Price","Price is too high");
                    return Page();
                }

                if(ImageFile!=null&&ImageFile.Length>0)
                {
                    string imagePath=await fileUploadService.UploadImageAsync(ImageFile,"artworks");
                    NewArtwork.ImageUrl=imagePath;
                }
                else
                {
                    NewArtwork.ImageUrl="/images/artworks/default.jpg";
                }

                artworkService.AddArtwork(NewArtwork);
                logger.LogInformation("Artwork added: {Title}",NewArtwork.Title);
                
                TempData["SuccessMessage"]=$"Artwork '{NewArtwork.Title}' added successfully!";
                return RedirectToPage("/Index");
            }
            catch(ArgumentException ex)
            {
                logger.LogWarning(ex,"Validation error adding artwork");
                ModelState.AddModelError("",ex.Message);
                return Page();
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"Error adding artwork");
                ModelState.AddModelError("","An error occurred while adding the artwork. Please try again.");
                return Page();
            }
        }
    }
}

