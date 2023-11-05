using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using ViewModel.Order;

namespace DrawClient.Pages.Admin.Customer
{
    public class OrderModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public OrderViewModel order { get; set; }

        public OrderModel(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _httpClient.BaseAddress = new Uri(apiUrl);
        }
        public async Task<IActionResult> OnGetAsync(int orderId, int customerId)
        {
            var token = HttpContext.Session.GetString("adminToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseOrder = _httpClient.GetAsync(_httpClient.BaseAddress + "/order/history-of-learner?learnerId=" + customerId).Result;
            if (responseOrder.IsSuccessStatusCode)
            {
                string data = responseOrder.Content.ReadAsStringAsync().Result;
                var rs = JsonConvert.DeserializeObject<List<OrderViewModel>>(data);
                order = rs.FirstOrDefault(o => o.Id == orderId);
            }
            return Page();
        }
    }
}
