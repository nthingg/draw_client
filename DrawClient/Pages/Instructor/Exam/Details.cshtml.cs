using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ViewModel.Exam;

namespace DrawClient.Pages.Instructor.Exam
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

        public ExamViewModel Exam { get; set; }
        [BindProperty]
        public ExamGradeViewModel ExamGrade { get; set; }
        [BindProperty]
        public int Id { get; set; }

        public async Task OnGetAsync(int id)
        {
            Id = id;
            await GetExamDetailAsync(id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataStr = JsonConvert.SerializeObject(ExamGrade);
                var content = new StringContent(dataStr, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Patch, _client.BaseAddress + "/exam/grade/" + Id);
                //
                var token = HttpContext.Session.GetString("instructToken");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = content;
                //
                var res = await _client.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    return Redirect("/Instructor/Exam/Grade");
                }
                else if (res.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["ResultError"] = "Grade only valid from 0 to 10";
                }
            }

            await GetExamDetailAsync(Id);
            return Page();
        }

        private async Task GetExamDetailAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/exam/result/" + id);
            var token = HttpContext.Session.GetString("instructToken");
            request.Headers.Add("Authorization", $"Bearer {token}");
            //
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var exam = JsonConvert.DeserializeObject<ExamViewModel>(dataStr);
                if (exam is not null)
                {
                    Exam = exam;
                }
            }
        }
    }
}
