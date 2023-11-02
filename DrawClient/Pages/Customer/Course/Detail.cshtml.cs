using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PayPal.Api;
using System.Text;
using ViewModel.Base;
using ViewModel.Cart;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Order;
using ViewModel.Review;

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

        public OrderDetail Detail { get; set; }

        public List<CourseViewModel> Purchased { get; set; }

        public List<LessonViewModel> Exams { get; set; }

        public async Task OnGetAsync(int id)
        {
            await Refresh(id);
            await SetOrderDetail();
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
            await SetOrderDetail();
            return Page();
        }

        public async Task<IActionResult> OnPostComment(int id, int detailId, string stars, string comment)
        {
            string learner = HttpContext.Session.GetString("learnerLogged");
            if (learner != "logged")
            {
                return RedirectToPage("/Customer/Authentication/Login");
            }

            

            var review = new ReviewCreateViewModel
            {
                Rating = double.Parse(stars),
                Comment = comment,
            };

            var dataStr = JsonConvert.SerializeObject(review);
            var content = new StringContent(dataStr, Encoding.UTF8, "application/json");

            var learnerToken = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Patch, _client.BaseAddress + "/order/rating/" + detailId);
            request.Headers.Add("Authorization", $"Bearer {learnerToken}");
            request.Content = content;

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {

            }

            await Refresh(id);
            await SetOrderDetail();
            return Page();
        }

        private async Task SetOrderDetail()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/order/history?pageIndex=0&pageSize=100");
            var res = await _client.SendAsync(request);


            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<Page<OrderViewModel>>(dataStr);

                int? learnerId = HttpContext.Session.GetInt32("learnerId");

                if (orders is not null)
                {   
                    OrderViewModel? order = orders.Items.FirstOrDefault(o => o.LearnerId == learnerId 
                    && o.Details?.FirstOrDefault(d => d.CourseId == Course.Id) != null
                    && o.Details.FirstOrDefault(d => d.CourseId == Course.Id).CourseId == Course.Id);

                    if (order is not null)
                    {
                        OrderDetail? detail = order.Details.FirstOrDefault(d => d.CourseId == Course.Id);

                        if (detail is not null)
                        {
                            Detail = detail;
                        }
                    }
                }
            }
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
