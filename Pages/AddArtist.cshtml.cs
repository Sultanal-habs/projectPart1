using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class AddArtistModel : PageModel
    {
        private readonly ArtistService artistService;
        private readonly FileUploadService fileUploadService;
        private readonly ILogger<AddArtistModel> logger;

        public AddArtistModel(ArtistService artistService,FileUploadService fileUploadService,ILogger<AddArtistModel> logger)
        {
            this.artistService =artistService;
            this.fileUploadService=fileUploadService;
            this.logger=logger;
        }

        [BindProperty]
        public Artist NewArtist{get;set;}=new Artist();

        [BindProperty]
        public IFormFile ProfileImageFile{get;set;}

        public void OnGet(){}

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                bool isValid=ModelState.IsValid;
                if(!isValid)
                {
                    return Page();
                }

                if(ProfileImageFile!=null&&ProfileImageFile.Length>0)
                {
                    string imagePath=await fileUploadService.UploadImageAsync(ProfileImageFile,"artists");
                    NewArtist.ProfileImageUrl=imagePath;
                }
                else
                {
                    NewArtist.ProfileImageUrl="/images/artists/default.jpg";
                }

                artistService.AddArtist(NewArtist);
                logger.LogInformation("Artist {Name} added successfully",NewArtist.Name);
                
                TempData["SuccessMessage"]=$"Artist profile for {NewArtist.Name} created successfully!";
                return RedirectToPage("/Artists");
            }
            catch(InvalidOperationException ex)
            {
                logger.LogWarning(ex,"Invalid operation while adding artist");
                ModelState.AddModelError("",ex.Message);
                return Page();
            }
            catch(ArgumentException ex)
            {
                logger.LogWarning(ex,"Invalid argument while adding artist");
                ModelState.AddModelError("",ex.Message);
                return Page();
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"Error adding artist");
                ModelState.AddModelError("","An unexpected error occurred. Please try again.");
                return Page();
            }
        }
    }
}
