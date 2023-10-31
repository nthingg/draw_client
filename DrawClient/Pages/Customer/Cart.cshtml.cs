using FUFlowerBouquetSystem_ThinhNN.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Course;

namespace DrawClient.Pages.Customer
{
    public class CartModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public CartModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public List<CourseViewModel> Courses { get; set; }

        public decimal Total { get; set; }

        public async Task<IActionResult> OnGetAsync(int removeId = 0)
        {
            if (removeId != 0)
            {
                await RemoveItem(removeId);
            }
            await GetCart();
            HttpContext.Session.SetInt32("cartQty", Courses.Count);

            if (Courses == null)
            {
                Total = 0;
            } else
            {
                foreach (var course in Courses)
                {
                    if (course.DiscountPrice != 0)
                    {
                        Total += course.DiscountPrice;
                    }
                    else
                    {
                        Total += course.OriginalPrice;
                    }
                }
            }
            return Page();
        }

        public async Task OnPostAsync()
        {

        }

        private async Task GetCart()
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/order/cart");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    Courses = courses;
                }
            }
        }

        private async Task RemoveItem(int id)
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + "/order/remove-from-cart?detailId=" + id);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
            }
        }
    }
}
