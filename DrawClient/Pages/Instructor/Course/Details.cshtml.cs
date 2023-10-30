using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Topic;

namespace DrawClient.Pages.Instructor.Course
{
    public class DetailsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public DetailsModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public CourseViewModel Course { get; set; }
        public Page<LessonViewModel> Lesson { get; set; }

        public async Task OnGetAsync(int id, int pageIndex = 0)
        {
            var res = await _client.GetAsync(_client.BaseAddress + "/course/" + id);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseViewModel>(dataStr);
                if (course is not null)
                {
                    Course = course;
                    Lesson = LessonPage(course.Lessons, pageIndex);
                    await GenerateLevelOptions();
                    await GenerateMaterialOptions();
                    await GenerateTopicOptions();
                }
            }
        }

        //
        private Page<LessonViewModel> LessonPage(ICollection<LessonViewModel> lessons, int pageIndex = 0, int pageSize = 4)
        {
            var totalCount = lessons.Count;
            var items = lessons
                .Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return new Page<LessonViewModel>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = totalCount
            };
        }
    }
}