using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Topic;

namespace DrawClient.Pages.Instructor.Course
{
    public class AddLessonModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _environment;

        public AddLessonModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public CourseViewModel Course { get; set; }
        [BindProperty]
        public LessonCreateViewModel Lesson { get; set; }
        [BindProperty]
        public IFormFile? VideoUrl { get; set; }
        [BindProperty]
        public string CourseName { get; set; }
        [BindProperty]
        public int Id { get; set; }
        public SelectList Topic { get; set; }
        public SelectList Material { get; set; }
        public SelectList Level { get; set; }

        public async Task OnGetAsync(string name)
        {
            CourseName = name;
            await GetCourseByNameAsync(name);
            await GenerateLevelOptions();
            await GenerateMaterialOptions();
            await GenerateTopicOptions();
            //
            Id = Course.Id;
        }

        public async Task<IActionResult> OnGetFinishAsync(int id)
        {
            var token = HttpContext.Session.GetString("instructToken");
            var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + "/course/status/" + id);
            request.Headers.Add("Authorization", $"Bearer {token}");
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                return Redirect("/Instructor/Course");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (VideoUrl is not null)
                {
                    // Get the full path to the wwwroot directory.
                    string wwwrootPath = _environment.WebRootPath;

                    Guid name = Guid.NewGuid();
                    var filePath = Path.Combine(wwwrootPath, "images", "course", "videos", name.ToString() + Path.GetExtension(VideoUrl.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await VideoUrl.CopyToAsync(fileStream);
                    }

                    Lesson.VideoUrl = @"/images/course/videos/" + name.ToString() + Path.GetExtension(VideoUrl.FileName);
                }
                var dataStr = JsonConvert.SerializeObject(Lesson);
                var content = new StringContent(dataStr, Encoding.UTF8, "application/json");

                //
                var res = await _client.PostAsync(_client.BaseAddress + "/course/lesson/" + Id, content);
                if (res.IsSuccessStatusCode)
                {
                    TempData["succeed"] = "Add Succeed";
                    return RedirectToPage("/Instructor/Course/AddLesson", new { name = CourseName });
                }
            }

            await GetCourseByNameAsync(CourseName);
            await GenerateLevelOptions();
            await GenerateMaterialOptions();
            await GenerateTopicOptions();

            return Page();
        }
        //
        private async Task GetCourseByNameAsync(string name)
        {
            var res = await _client.GetAsync(_client.BaseAddress + $"/course/lastest-by-name?name={name}");
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseViewModel>(dataStr);
                if (course is not null)
                {
                    Course = course;
                }
            }
        }

        private async Task GenerateTopicOptions()
        {
            var res = await _client.GetAsync(_client.BaseAddress + "/base/topic");
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var topics = JsonConvert.DeserializeObject<List<TopicBaseViewModel>>(dataStr);
                Topic = new SelectList(topics, nameof(TopicBaseViewModel.Id), nameof(TopicBaseViewModel.Name), Course.Topic.Id);
            }
        }

        private async Task GenerateLevelOptions()
        {
            var res = await _client.GetAsync(_client.BaseAddress + "/base/level");
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var level = JsonConvert.DeserializeObject<List<TopicBaseViewModel>>(dataStr);
                Level = new SelectList(level, nameof(TopicBaseViewModel.Id), nameof(TopicBaseViewModel.Name), Course.Level);
            }
        }

        private async Task GenerateMaterialOptions()
        {
            var res = await _client.GetAsync(_client.BaseAddress + "/base/material");
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var materials = JsonConvert.DeserializeObject<List<TopicBaseViewModel>>(dataStr);
                Material = new SelectList(materials, nameof(TopicBaseViewModel.Id), nameof(TopicBaseViewModel.Name), Course.Material);
            }
        }
    }
}
