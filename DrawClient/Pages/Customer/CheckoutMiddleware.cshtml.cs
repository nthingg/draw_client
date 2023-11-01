using DrawClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PayPal.Api;
using System.Text;
using ViewModel.Order;

namespace DrawClient.Pages.Customer
{
    public class CheckoutMiddlewareModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public CheckoutMiddlewareModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public async Task<IActionResult> OnGet(string paymentId, string token, string PayerID)
        {
            var payment = new PaymentViewModel
            {
                TransactionId = paymentId,
                PaymentMethod = 0,
            };

            var dataStr = JsonConvert.SerializeObject(payment);
            var content = new StringContent(dataStr, Encoding.UTF8, "application/json");

            var learnerToken = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Patch, _client.BaseAddress + "/order/complete");
            request.Headers.Add("Authorization", $"Bearer {learnerToken}");
            request.Content = content;

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                HttpContext.Session.SetInt32("cartQty", 0);
                return Redirect("/Customer/Course/Purchased");
            }
            return Redirect("/Customer/Course");
        }
    }
}
