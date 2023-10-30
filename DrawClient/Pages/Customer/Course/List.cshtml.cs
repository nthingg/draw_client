using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using ViewModel.Base;
using ViewModel.Course;

namespace DrawClient.Pages.Customer.Course
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

        public Page<CourseViewModel> Courses { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 0)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/page?pageIndex={pageIndex}&pageSize={5}");
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
            return Page();
        }
    }
}
