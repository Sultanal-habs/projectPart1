using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projectPart1.Models;
using projectPart1.Services;

namespace projectPart1.Pages
{
    public class AddExhibitionModel : PageModel
    {
        private readonly ExhibitionService exhibitionService;
        private readonly FileUploadService fileUploadService;
        private readonly ILogger<AddExhibitionModel> logger;

        public AddExhibitionModel(ExhibitionService exhibitionService,FileUploadService fileUploadService,ILogger<AddExhibitionModel> logger)
        {
            this.exhibitionService=exhibitionService;
            this.fileUploadService=fileUploadService;
            this.logger =logger;
        }

        [BindProperty]
        public Exhibition NewExhibition{get;set;}=new Exhibition();

        [BindProperty]
        public IFormFile BannerImageFile{get;set;}

        public void OnGet()
        {
            DateTime today=DateTime.Today;
            NewExhibition.StartDate=today.AddDays(7);
            NewExhibition.EndDate =today.AddDays(37);
        }

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

                DateTime today=DateTime.Today;
                if(NewExhibition.StartDate<today)
                {
                    ModelState.AddModelError("NewExhibition.StartDate","Start date must be in the future");
                    return Page();
                }

                if(NewExhibition.EndDate<=NewExhibition.StartDate)
                {
                    ModelState.AddModelError("NewExhibition.EndDate","End date must be after start date");
                    return Page();
                }

                TimeSpan diff=NewExhibition.EndDate-NewExhibition.StartDate;
                int days=diff.Days;
                
                if(days<1)
                {
                    ModelState.AddModelError("","Exhibition must be at least one day");
                    return Page();
                }

                if(days>365)
                {
                    ModelState.AddModelError("","Exhibition cannot be longer than one year");
                    return Page();
                }

                if(BannerImageFile!=null&&BannerImageFile.Length>0)
                {
                    string imagePath=await fileUploadService.UploadImageAsync(BannerImageFile,"exhibitions");
                    NewExhibition.BannerImageUrl=imagePath;
                }
                else
                {
                    NewExhibition.BannerImageUrl="/images/exhibitions/default.jpg";
                }

                exhibitionService.AddExhibition(NewExhibition);
                logger.LogInformation("Exhibition added: {Name}",NewExhibition.Name);
                
                TempData["SuccessMessage"]=$"Exhibition '{NewExhibition.Name}' created successfully!";
                return RedirectToPage("/Exhibitions");
            }
            catch(ArgumentException ex)
            {
                logger.LogWarning(ex,"Validation error adding exhibition");
                ModelState.AddModelError("",ex.Message);
                return Page();
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"Error adding exhibition");
                ModelState.AddModelError("","An error occurred. Please try again.");
                return Page();
            }
        }
    }
}
