using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class UpdateModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        [BindProperty]
        public InstructorViewModel instructor { get; set; }
        public UpdateModel()
        {
            _httpClient = new HttpClient();
        }
        public async Task<IActionResult> OnGet(int id) 
        {
            var log = HttpContext.Session.GetString("adminLogged");
            if (log == "logged")
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
            else
            {
                TempData["error"] = "Please login with admin role to access this url!";
                return RedirectToPage("/Admin/Authentication/Login");
            }
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
				var token = HttpContext.Session.GetString("adminToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				string data = JsonConvert.SerializeObject(instructor);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync(baseAddress + "instructor/" + instructor.Id, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Instructor information updated!";
                    return RedirectToPage("/Admin/Instructor/List");
                }
                else
                {
                    string message = "An error has occured";
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JsonDocument doc = JsonDocument.Parse(responseContent);
                    if (response.StatusCode == HttpStatusCode.BadRequest && responseContent.Length > 0)
                    {
                        var errors = doc.RootElement.GetProperty("errors").ToString();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        var validateResults = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errors, options);
                        foreach (var key in validateResults.Keys)
                        {
                            var errorMessage = string.Join(" ", validateResults[key]);
                            ModelState.AddModelError("Instructor." + key, errorMessage);
                        }
                    }
                }
            }
            return Page();
        }
    }
}
