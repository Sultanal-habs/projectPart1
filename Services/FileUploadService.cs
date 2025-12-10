using Microsoft.AspNetCore.Http;
namespace projectPart1.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment environment;
        public FileUploadService(IWebHostEnvironment env)
        {
            environment=env;
        }
        public async Task<string> UploadImageAsync(IFormFile file,string folder)
        {
            if(file==null||file.Length==0)
            {
                return null;
            }
            string[] allowedExtensions={".jpg",".jpeg",".png",".gif"};
            string fileExtension=Path.GetExtension(file.FileName).ToLower();
            bool isValid=false;
            for(int i=0;i<allowedExtensions.Length;i++)
            {
                if(fileExtension==allowedExtensions[i])
                {
                    isValid=true;
                    break;
                }
            }
            if(!isValid)
            {
                throw new ArgumentException("Only image files are allowed (jpg, jpeg, png, gif)");
            }
            if(file.Length>5*1024*1024)
            {
                throw new ArgumentException("File size must be less than 5MB");
            }
            string uploadsFolder=Path.Combine(environment.WebRootPath,"images",folder);
            if(!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName=Guid.NewGuid().ToString()+fileExtension;
            string filePath=Path.Combine(uploadsFolder,uniqueFileName);
            using(FileStream stream=new FileStream(filePath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/images/{folder}/{uniqueFileName}";
        }
        public bool DeleteImage(string imagePath)
        {
            if(string.IsNullOrEmpty(imagePath))
            {
                return false;
            }
            try
            {
                string fullPath=Path.Combine(environment.WebRootPath,imagePath.TrimStart('/'));
                if(File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}