using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class CreateModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        [BindProperty]
        public InstructorBaseViewModel Instructor { get; set; }
        public CreateModel()
        {
            _httpClient = new HttpClient();
            Instructor = new InstructorBaseViewModel();
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(InstructorBaseViewModel instructor)
        {

            if(ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(instructor);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(baseAddress + "instructor/register", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Admin/Instructor/List");
                }
            }
            return Page();
        }
    }
}
