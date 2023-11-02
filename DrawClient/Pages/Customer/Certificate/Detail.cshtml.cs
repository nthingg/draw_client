using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Course;
using ViewModel.Learner;

namespace DrawClient.Pages.Customer.Certificate
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

        public List<PurchasedCourseViewModel> Purchased { get; set; }

        public PurchasedCourseViewModel Certificate { get; set; }

        public LearnerBaseViewModel Learner { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/course/purchased-course");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<PurchasedCourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    Purchased = courses;
                }

                Certificate = Purchased.FirstOrDefault(c => c.Id == id);
                await GetLearner();
            }
            return Page();
        }

        private async Task GetLearner()
        {
            var id = HttpContext.Session.GetString("learnerId");

            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/learner/" + id);

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var learner = JsonConvert.DeserializeObject<LearnerBaseViewModel>(dataStr);
                if (learner is not null)
                {
                    Learner = learner;
                }
            }
        }
    }
}
