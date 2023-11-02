using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Course;

namespace DrawClient.Pages.Customer.Certificate
{
    public class ListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public ListModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public List<PurchasedCourseViewModel> Passed { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/purchased-course");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var purchased = JsonConvert.DeserializeObject<List<PurchasedCourseViewModel>>(dataStr);
                if (purchased is not null)
                {
                    Passed = new List<PurchasedCourseViewModel>();
                    foreach (var item in purchased)
                    {
                        var course = await GetCourseById(item.Id);
                        if (course is not null)
                        {
                            if (course.HasPassed != false)
                            {
                                Passed.Add(course);
                            }
                        }
                    }
                }
            }
            return Page();
        }

        private async Task<PurchasedCourseViewModel> GetCourseById(int id)
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/" + id);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<PurchasedCourseViewModel>(dataStr);
                return course;
            }
            return null;
        }
    }   
}
