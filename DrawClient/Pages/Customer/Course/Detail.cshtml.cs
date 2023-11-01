using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Cart;
using ViewModel.Course;
using ViewModel.Lesson;

namespace DrawClient.Pages.Customer.Course
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

        public List<CourseViewModel> Purchased { get; set; }

        public List<LessonViewModel> Exams { get; set; }

        public async Task OnGetAsync(int id)
        {
            await Refresh(id);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            string learner = HttpContext.Session.GetString("learnerLogged");
            if (learner != "logged")
            {
                return RedirectToPage("/Customer/Authentication/Login");
            }

            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + $"/order/add-to-cart?courseId=" + id);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                await SetCartQty();
            }
            await Refresh(id);
            return Page();
        }

        public async Task<IActionResult> OnPostComment(int id)
        {
            string learner = HttpContext.Session.GetString("learnerLogged");
            if (learner != "logged")
            {
                return RedirectToPage("/Customer/Authentication/Login");
            }

            return Page();
        }

        private async Task GetPurchasesCourse()
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/purchased-course");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    Purchased = courses;
                }
            }
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
                await GetPurchasesCourse();
                Exams = Course.Lessons.Where(lesson => lesson.IsExam).ToList();
            }
        }

        private async Task SetCartQty()
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/order/cart");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<List<OrderDetailViewModel>>(dataStr);
                if (details is not null)
                {
                    HttpContext.Session.SetInt32("cartQty", details.Count);
                }
            }
        }
    }
}
