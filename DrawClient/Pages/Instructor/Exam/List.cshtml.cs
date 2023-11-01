using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Exam;
using ViewModel.Lesson;

namespace DrawClient.Pages.Instructor.Exam
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

        public ICollection<ExamViewModel> Exams { get; set; }
        public LessonViewModel Lesson { get; set; }

        public async Task OnGetAsync(int id)
        {
            await GetLessonDetailAsync(id);
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/exam/by-lesson-id/" + id);
            var token = HttpContext.Session.GetString("instructToken");
            request.Headers.Add("Authorization", $"Bearer {token}");
            //
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var exams = JsonConvert.DeserializeObject<ICollection<ExamViewModel>>(dataStr);
                if (exams is not null)
                {
                    Exams = exams;
                }
            }
        }

        private async Task GetLessonDetailAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/lesson/" + id);
            //
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var lesson = JsonConvert.DeserializeObject<LessonViewModel>(dataStr);
                if (lesson is not null)
                {
                    Lesson = lesson;
                }
            }
        }
    }
}
