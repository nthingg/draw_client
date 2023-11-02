using AutoMapper;
using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ViewModel.Base;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Topic;

namespace DrawClient.Pages.Instructor.Course
{
    public class UpdateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly CloudinaryHelper _cloudinary;
        private readonly IMapper _mapper;

        public UpdateModel(IConfiguration configuration, CloudinaryHelper cloudinary, IMapper mapper)
        {
            _configuration = configuration;
            _cloudinary = cloudinary;
            _mapper = mapper;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public CourseUpdateViewModel CourseUpdate { get; set; }
        [BindProperty]
        public IFormFile? Image { get; set; }

        //
        [BindProperty]
        public LessonCreateViewModel LessonUpdate { get; set; }
        [BindProperty]
        public IFormFile? VideoUrl { get; set; }
        [BindProperty]
        public int LessonId { get; set; }

        //
        public CourseViewModel Course { get; set; }
        public Page<LessonViewModel> Lesson { get; set; }
        public SelectList Topic { get; set; }
        public SelectList Material { get; set; }
        public SelectList Level { get; set; }

        public async Task OnGetAsync(int id, int pageIndex = 0)
        {
            Id = id;
            await GetCourseDetailAsync(Id, pageIndex);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove(nameof(LessonUpdate));
            ModelState.Remove(nameof(LessonId));
            ModelState.Remove(nameof(VideoUrl));
            ModelState.Remove("Content");
            ModelState.Remove("Title");
            if (ModelState.IsValid)
            {
                if (Image is not null)
                {
                    var imgUrl = await _cloudinary.UploadImageToCloudinaryAsync(Image);
                    CourseUpdate.ThumbUrl = imgUrl;
                }

                if (CourseUpdate.DiscountPrice == 0)
                {
                    CourseUpdate.DiscountPrice = null;
                }

                var dataStr = JsonConvert.SerializeObject(CourseUpdate);
                var content = new StringContent(dataStr, Encoding.UTF8, "application/json");
                //
                var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "/course/" + Id);
                var token = HttpContext.Session.GetString("instructToken");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = content;
                //
                var res = await _client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    TempData["success"] = "Update Succeed";
                    await GetCourseDetailAsync(Id, 0);
                    return Page();
                }
            }

            TempData["error"] = "Error";
            await GetCourseDetailAsync(Id, 0);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLessonAsync(int? pageIndex, string? isExamResult)
        {
            if (isExamResult is not null && isExamResult == "on")
            {
                LessonUpdate.IsExam = true;
            }
            ModelState.Remove(nameof(CourseUpdate));
            ModelState.Remove(nameof(Image));
            ModelState.Remove(nameof(CourseUpdate.Name));
            if (ModelState.IsValid)
            {
                if (VideoUrl is not null)
                {
                    var imgUrl = await _cloudinary.UploadImageToCloudinaryAsync(VideoUrl);
                    LessonUpdate.VideoUrl = imgUrl;
                }

                var dataStr = JsonConvert.SerializeObject(LessonUpdate);
                var content = new StringContent(dataStr, Encoding.UTF8, "application/json");
                //
                var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "/course/lesson/" + LessonId);
                var token = HttpContext.Session.GetString("instructToken");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = content;
                //
                var res = await _client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    TempData["success"] = "Update Succeed";
                    await GetCourseDetailAsync(Id, pageIndex ?? 0);
                    return Page();
                }
            }

            TempData["error"] = "Error";
            await GetCourseDetailAsync(Id, pageIndex ?? 0);
            return Page();
        }

        public async Task<IActionResult> OnGetRemoveLessonAsync(int lessonId, int courseId)
        {
            var course = await GetCourseByIdAsync(courseId);
            var examCount = 0;
            if (course is not null)
            {
                foreach (var item in course.Lessons)
                {
                    if (item.Id != lessonId && item.IsExam)
                    {
                        examCount++;
                    }
                }
            }
            //
            if (examCount != 0)
            {
                //
                var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + "/course/lesson/" + lessonId);
                var token = HttpContext.Session.GetString("instructToken");
                request.Headers.Add("Authorization", $"Bearer {token}");
                //
                var res = await _client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    TempData["success"] = "Remove Succeed";
                    Id = courseId;
                    await GetCourseDetailAsync(courseId, 0);
                    return Page();
                }
                else
                {
                    TempData["error"] = "Error";
                }
            }
            else
            {
                TempData["error"] = "Your course need at least 1 exam";
            }

            Id = courseId;
            await GetCourseDetailAsync(courseId, 0);
            return Page();
        }

        public async Task<IActionResult> OnGetRemoveCourseAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + "/course/status/" + id);
            var token = HttpContext.Session.GetString("instructToken");
            request.Headers.Add("Authorization", $"Bearer {token}");
            //
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                TempData["success"] = "Removed Succeed";
                return Redirect("/Instructor/Course");
            }

            TempData["error"] = "Error";
            await GetCourseDetailAsync(id, 0);
            return Page();
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

        private async Task<CourseViewModel?> GetCourseByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/" + id);
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseViewModel>(dataStr);
                return course;
            }

            return null;
        }

        private async Task GetCourseDetailAsync(int id, int pageIndex)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/" + id);
            var token = HttpContext.Session.GetString("instructToken");
            request.Headers.Add("Authorization", $"Bearer {token}");
            //
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var course = JsonConvert.DeserializeObject<CourseViewModel>(dataStr);
                if (course is not null)
                {
                    Course = course;
                    Lesson = LessonPage(course.Lessons.OrderBy(l => l.Id).ToList(), pageIndex);
                    await GenerateLevelOptions();
                    await GenerateMaterialOptions();
                    await GenerateTopicOptions();
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
