using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
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

        public UpdateModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public CourseViewModel Course { get; set; }
        public Page<LessonViewModel> Lesson { get; set; }
        public SelectList Topic { get; set; }
        public SelectList Material { get; set; }
        public SelectList Level { get; set; }

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
