using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Course;

namespace DrawClient.Pages.Customer.Course
{
    public class PurchasedModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public PurchasedModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public List<CourseViewModel> Courses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/purchased-course");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    Courses = courses;
                }
            }
            return Page();
        }
    }
}
