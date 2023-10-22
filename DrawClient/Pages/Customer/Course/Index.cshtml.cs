using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrawClient.Pages.Customer.Course
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public string YtbURL {  get; set; }

        public void OnGet()
        {
            YtbURL = "https://www.youtube.com/embed/tgbNymZ7vqY";
        }
    }
}
