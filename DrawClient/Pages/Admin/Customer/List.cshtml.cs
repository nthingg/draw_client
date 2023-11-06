using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Base;
using ViewModel.Instructor;
using ViewModel.Learner;

namespace DrawClient.Pages.Admin.Customer
{
    public class ListModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public Page<LearnerBaseViewModel> Learners { get; set; }
        public ListModel()
        {
            _httpClient = new HttpClient();
            Learners = new Page<LearnerBaseViewModel>();
        }
        public async Task<IActionResult> OnGetAsync(int pageIndex = 0)
        {
            var log = HttpContext.Session.GetString("adminLogged");
            if(log == "logged")
            {
                var token = HttpContext.Session.GetString("adminToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = _httpClient.GetAsync(baseAddress + $"learner/page?pageIndex={pageIndex}&pageSize=10").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Learners = JsonConvert.DeserializeObject<Page<LearnerBaseViewModel>>(data);
                }
                return Page();
            }
            else
            {
                TempData["error"] = "Please log in with admin role to access this url!";
                return RedirectToPage("/Admin/Authentication/Login");
            }
			
        }
    }
}
