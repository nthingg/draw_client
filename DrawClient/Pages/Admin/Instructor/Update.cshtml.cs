using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class UpdateModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public InstructorViewModel instructor { get; set; }
        public UpdateModel()
        {
            _httpClient = new HttpClient();
        }
        public async Task<IActionResult> OnGet(int id) 
        {
            HttpResponseMessage response = _httpClient.GetAsync(baseAddress + "instructor/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                instructor = JsonConvert.DeserializeObject<InstructorViewModel>(data);
            }
            return Page();
        }
    }
}
