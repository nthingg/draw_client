using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrawClient.Pages.Customer.Course
{
    public class IndexModel : PageModel
    {
        private readonly CloudinaryHelper cloudinary;

        public IndexModel(CloudinaryHelper cloudinary)
        {
            this.cloudinary = cloudinary;
        }

        [BindProperty]
        public string YtbURL {  get; set; }

        [BindProperty]
        public string ImageURl { get; set; }

        [BindProperty]
        public IFormFile? Image { get; set; }

        public void OnGet()
        {
            YtbURL = "https://www.youtube.com/embed/tgbNymZ7vqY";
            ImageURl = "https://i.pinimg.com/originals/c3/ac/09/c3ac09a329ca1a4dbbbb8620a26a57ca.jpg";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var imageUrl = await cloudinary.UploadImageToCloudinaryAsync(Image);
            return Page();
        }
    }
}
