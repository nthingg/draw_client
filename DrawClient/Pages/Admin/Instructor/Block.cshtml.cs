using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class BlockModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        [BindProperty]
        public InstructorViewModel instructor { get; set; }
        public BlockModel()
        {
            _httpClient = new HttpClient(); 
        }
        public async Task<IActionResult> OnGet(int id)
        {
			var token = HttpContext.Session.GetString("adminToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = _httpClient.GetAsync(baseAddress + "instructor/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                instructor = JsonConvert.DeserializeObject<InstructorViewModel>(data);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			var token = HttpContext.Session.GetString("adminToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = _httpClient.DeleteAsync(baseAddress + "instructor/status?id=" + instructor.Id).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/Instructor/List");
            }
            return Page();
        }
    }
}
