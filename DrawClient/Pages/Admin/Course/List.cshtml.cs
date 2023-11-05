using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Cart;
using ViewModel.Course;

namespace DrawClient.Pages.Admin.Course
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
            await Refresh(pageIndex);
            return Page();
        }

        private async Task Refresh(int pageIndex = 0)
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/page?pageIndex={pageIndex}&pageSize={6}");
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
