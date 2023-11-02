using DrawClient.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PayPal.Api;
using System.Text;
using ViewModel.Cart;
using ViewModel.Order;

namespace DrawClient.Pages.Customer
{
    public class CartModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IPaypalServices _paypalServices;

        public CartModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
            _paypalServices = new PaypalServices(_configuration);
        }

        public List<OrderDetailViewModel> Details { get; set; }

        public decimal? Total { get; set; }

        public async Task<IActionResult> OnGetAsync(int removeId = 0)
        {
            string? learnerLogged = HttpContext.Session.GetString("learnerLogged");
            if (learnerLogged != "logged")
            {
                return Redirect("/Customer/Authentication/Login");
            }

            if (removeId != 0)
            {
                await RemoveItem(removeId);
            }
            await GetCart();
            if (Details is not null)
            {
                HttpContext.Session.SetInt32("cartQty", Details.Count);
            } else
            {
                HttpContext.Session.SetInt32("cartQty", 0);
            }

            Total = 0;
            if (Details != null)
            {
                foreach (var detail in Details)
                {
                    if (detail.Course.DiscountPrice is not null)
                    {
                        Total += detail.Course.DiscountPrice;
                    }
                    else
                    {
                        Total += detail.Course.OriginalPrice;
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(decimal total)
        {
            try
            { 
                if (total > 0)
                {
                    decimal amount = ConvertVNDToUSD(total);
                    string returnUrl = "https://localhost:7236/Customer/CheckoutMiddleware";
                    string cancelUrl = "https://localhost:7236/Customer/Cart";

                    //Create a paypal order
                    var createPayment = await _paypalServices.CreateOrderAsync(amount, returnUrl, cancelUrl);

                    //Get paypal approve url
                    string approveUrl = createPayment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href;

                    //Redirect to url
                    if (!string.IsNullOrEmpty(approveUrl))
                    {
                        return Redirect(approveUrl);
                    }
                } else
                {
                    var payment = new PaymentViewModel
                    {
                        TransactionId = "FREE",
                        PaymentMethod = 1,
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
            } catch (Exception ex) 
            { 

            }

            return Redirect("Cart");
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
                var details = JsonConvert.DeserializeObject<List<OrderDetailViewModel>>(dataStr);
                if (details is not null)
                {
                    Details = details;
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

        private decimal ConvertVNDToUSD(decimal vndAmount)
        {
            // Use a fixed exchange rate (example rate, you should replace with the actual rate)
            decimal exchangeRate = 0.000043M;  // 1 VND to USD exchange rate

            // Convert VND to USD
            decimal usdAmount = vndAmount * exchangeRate;

            return usdAmount;
        }

    }
}
