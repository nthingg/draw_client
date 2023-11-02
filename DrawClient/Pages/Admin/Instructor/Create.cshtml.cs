using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class CreateModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        [BindProperty]
        public InstructorSignUpViewModel Instructor { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }
        public CreateModel()
        {
            _httpClient = new HttpClient();
          
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(InstructorSignUpViewModel instructor)
        {
            if (Instructor.Password != ConfirmPassword)
            {
                TempData["error"] = "Password and Confirm password must be the same!";
                return Page();
            }
            if (ModelState.IsValid)
            {
				var token = HttpContext.Session.GetString("adminToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
