using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrawClient.Pages.Customer.Exam
{
    public class DetailModel : PageModel
    {
        [BindProperty]
        public string ImageURl { get; set; }

        public void OnGet()
        {
            ImageURl = "https://i.pinimg.com/originals/c3/ac/09/c3ac09a329ca1a4dbbbb8620a26a57ca.jpg";
        }
    }
}
