using ImageToASCII.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ImageToASCII.Pages
{
    public class ConvertImageModel : PageModel
    {
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        [BindProperty]
        public string ImageUrl { get; set; }

        public string AsciiRepresentation { get; set; }

        public string ImageFilePath { get; set; }


        public void OnPost()
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Save the uploaded image to the wwwroot folder
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }

                // Set the image file path
                ImageFilePath = "/uploads/" + uniqueFileName;

                // Resize and convert the image
                using (var imageStream = ImageFile.OpenReadStream())
                {
                    var converter = new ImageConverter();
                    AsciiRepresentation = converter.ResizeImage(imageStream, rows: 50, method: 1);
                }
            }
        }


        public async Task<IActionResult> OnGetAscii(string imageUrl)
        {
            using (var webClient = new WebClient())
            {
                using (var imageStream = await webClient.OpenReadTaskAsync(imageUrl))
                {
                    var converter = new ImageConverter();
                    string asciiRepresentation = converter.ResizeImage(imageStream, rows: 50, method: 1);
                    return Content(asciiRepresentation, "text/plain");
                }
            }
        }

        public async Task<IActionResult> OnGetImage(string url)
        {
            using (var webClient = new WebClient())
            {
                byte[] imageData = await webClient.DownloadDataTaskAsync(url);
                return File(imageData, "image/jpeg"); // Adjust the content type based on the image format
            }
        }
    }
}
