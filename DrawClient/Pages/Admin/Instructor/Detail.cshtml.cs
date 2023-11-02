using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class DetailModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public InstructorViewModel instructor { get; set; }
        public DetailModel()
        {
            _httpClient = new HttpClient();

        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
			var token = HttpContext.Session.GetString("adminToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = _httpClient.GetAsync(baseAddress + "instructor/"+id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                instructor = JsonConvert.DeserializeObject<InstructorViewModel>(data);
            }
            return Page();
        }
    }
}
