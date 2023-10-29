using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Course;

namespace DrawClient.Pages.Instructor.Course
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public Page<CourseViewModel> Courses { get; set; }

        public async Task OnGetAsync(int pageIndex = 0)
        {
            var token = HttpContext.Session.GetString("instructToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/by-instructor?pageIndex={pageIndex}&pageSize={5}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<Page<CourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    Courses = courses;
                }
            }
        }
    }
}
