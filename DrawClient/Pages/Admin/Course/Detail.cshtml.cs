using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using ViewModel.Base;
using ViewModel.Cart;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Order;
using ViewModel.Review;

namespace DrawClient.Pages.Admin.Course
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

        public CourseViewModel Course { get; set; }

        public List<LessonViewModel> Exams { get; set; }

        public async Task OnGetAsync(int id)
        {
            await Refresh(id);
        }

        private async Task Refresh(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/" + id);
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseViewModel>(dataStr);
                Course = course;
                Exams = Course.Lessons.Where(lesson => lesson.IsExam).ToList();
            }
        }
    }
}
