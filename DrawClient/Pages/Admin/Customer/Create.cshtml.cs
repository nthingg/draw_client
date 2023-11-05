using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using ViewModel.Base;

namespace DrawClient.Pages.Admin.Customer
{
    public class CreateModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public SignUpViewModel learner { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _httpClient.BaseAddress = new Uri(apiUrl);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (learner.Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and confirm password must be the same!");
                return Page();
            }
            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("adminToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var dataStr = JsonConvert.SerializeObject(learner);
                var content = new StringContent(dataStr, System.Text.Encoding.UTF8, "application/json");
                var res = await _httpClient.PostAsync(_httpClient.BaseAddress + "/learner/register", content);
                if (res.IsSuccessStatusCode)
                {
                    return Redirect("/Admin/Customer/List");
                }
                else
                {
                    string message = "An error has occured";
                    string responseContent = await res.Content.ReadAsStringAsync();
                    JsonDocument doc = JsonDocument.Parse(responseContent);
                    if (res.StatusCode == HttpStatusCode.BadRequest && responseContent.Length > 0)
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
                            ModelState.AddModelError("Learner." + key, errorMessage);
                        }
                    }
                }
            }
            return Page();
        }
    }
}
