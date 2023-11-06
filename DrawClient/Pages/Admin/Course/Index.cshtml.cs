using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using ViewModel.Course;
using ViewModel.Exam;
using ViewModel.Lesson;

namespace DrawClient.Pages.Admin.Course
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly CloudinaryHelper cloudinary;

        public IndexModel(IConfiguration configuration, CloudinaryHelper cloudinary)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
            this.cloudinary = cloudinary;
        }

        [BindProperty]
        public IFormFile? Image { get; set; }

        public List<LessonViewModel> Lessons { get; set; } = new List<LessonViewModel>();

        public LessonViewModel Lesson { get; set; }

        public CourseViewModel Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int id = 0, int changeId = 0)
        {
            await Refresh(id);
            foreach (var item in Course.Lessons)
            {
                var lesson = await GetLessonById(item.Id);
                if (lesson == null) { continue; }
                else { Lessons.Add(lesson); }
            }

            if (changeId != 0)
            {
                Lesson = Lessons.FirstOrDefault(l => l.Id == changeId);
            }
            else
            {
                Lesson = Lessons.FirstOrDefault();
            }
            return Page();
        }

        private async Task Refresh(int id)
        {
            var learnerToken = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/" + id);
            request.Headers.Add("Authorization", $"Bearer {learnerToken}");

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseViewModel>(dataStr);
                Course = course;
                //Exams = Course.Lessons.Where(lesson => lesson.IsExam).ToList();
            }
        }

        private async Task<LessonViewModel> GetLessonById(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/lesson/" + id);
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var lesson = JsonConvert.DeserializeObject<LessonViewModel>(dataStr);
                return lesson;
                //Exams = Course.Lessons.Where(lesson => lesson.IsExam).ToList();
            }
            return null;
        }
    }
}
