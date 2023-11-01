using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Course;
using ViewModel.Instructor;

namespace DrawClient.Pages.Customer.Instructor
{
    public class DetailModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public DetailModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public InstructorViewModel Instructor { get; set; }

        public Page<CourseViewModel> Courses { get; set; }

        public async Task OnGetAsync(int id, int pageIndex = 0)
        {
            await Refresh(id, pageIndex);
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/instructor/" + id);
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var instructor = JsonConvert.DeserializeObject<InstructorViewModel>(dataStr);
                Instructor = instructor;
            }
        }

        private async Task Refresh(int id, int pageIndex = 0)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/by-instructor?instructorId={id}&pageIndex={pageIndex}&pageSize={6}");
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
