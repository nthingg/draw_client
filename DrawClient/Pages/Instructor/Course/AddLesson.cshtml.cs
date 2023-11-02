using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Topic;

namespace DrawClient.Pages.Instructor.Course
{
    public class AddLessonModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly CloudinaryHelper _cloudinary;

        public AddLessonModel(IConfiguration configuration, CloudinaryHelper cloudinary)
        {
            _configuration = configuration;
            _cloudinary = cloudinary;
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

        public async Task<IActionResult> OnGetFinishAsync(int id, string courseName)
        {
            await GetCourseByNameAsync(courseName);
            var isValidCourse = false;
            foreach (var item in Course.Lessons)
            {
                if (item.IsExam)
                {
                    isValidCourse = true;
                    break;
                }
            }
            if (isValidCourse)
            {
                var token = HttpContext.Session.GetString("instructToken");
                var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + "/course/status/" + id);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var res = await _client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    return Redirect("/Instructor/Course");
                }
            }
            else
            {
                Id = id;
                CourseName = courseName;
                TempData["error"] = "Your course need at least 1 exam";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? isExamResult, bool isUpdate = false, int? courseId = null)
        {
            if (isExamResult is not null || isExamResult == "on")
            {
                Lesson.IsExam = true;
            }
            if (isUpdate)
            {
                ModelState.Remove(nameof(Id));
                ModelState.Remove(nameof(CourseName));
            }
            if (ModelState.IsValid)
            {
                if (VideoUrl is not null)
                {
                    var url = await _cloudinary.UploadImageToCloudinaryAsync(VideoUrl);
                    Lesson.VideoUrl = url;
                }
                var dataStr = JsonConvert.SerializeObject(Lesson);
                var content = new StringContent(dataStr, Encoding.UTF8, "application/json");

                //
                var requestId = isUpdate ? courseId : Id;
                var token = HttpContext.Session.GetString("instructToken");
                var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/course/lesson/" + requestId);
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = content;

                var res = await _client.SendAsync(request);
                //
                if (res.IsSuccessStatusCode)
                {
                    TempData["success"] = "Add Succeed";
                    if (isUpdate)
                    {
                        return RedirectToPage("/Instructor/Course/Update", new { id = courseId });
                    }
                    return RedirectToPage("/Instructor/Course/AddLesson", new { name = CourseName });
                }
            }

            TempData["error"] = "Error";
            if (isUpdate)
            {
                return RedirectToPage("/Instructor/Course/Update", new { id = courseId });
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
            var token = HttpContext.Session.GetString("instructToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/lastest-by-name?name={name}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var res = await _client.SendAsync(request);
            //
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
