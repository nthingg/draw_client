using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PayPal.Api;
using System.Net.Http;
using System.Text;
using ViewModel.Base;
using ViewModel.Cart;
using ViewModel.Course;

namespace DrawClient.Pages.Customer.Course
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

        public Page<CourseViewModel> Courses { get; set; }

        public List<CourseViewModel> Purchased { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 0)
        {
            await Refresh(pageIndex);
            await GetPurchasesCourse();
            return Page();
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
            await Refresh();
            await GetPurchasesCourse();
            return Page();
        }

        public async Task<IActionResult> OnPostSearchAsync(string searchVal)
        {   
            var search = new SearchViewModel
            {
                Name = searchVal,
            };
            var dataStr = JsonConvert.SerializeObject(search);
            var content = new StringContent(dataStr, Encoding.UTF8, "application/json");

            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/course/search?pageIndex=0&pageSize=9");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Content = content;

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var rspStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<Page<CourseViewModel>>(rspStr);
                if (courses is not null)
                {
                    Courses = courses;
                }
            }
            return Page();
        }

        private async Task Refresh(int pageIndex = 0)
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/page?pageIndex={pageIndex}&pageSize={6}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<Page<CourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    Courses = courses;
                }
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

    }
}
