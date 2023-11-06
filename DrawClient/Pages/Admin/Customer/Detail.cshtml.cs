using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Instructor;
using ViewModel.Learner;
using ViewModel.Order;

namespace DrawClient.Pages.Admin.Customer
{
    public class DetailModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public LearnerBaseViewModel Learner { get; set; }   
        public List<OrderViewModel> orders { get; set; }
        public List<decimal> totals { get; set; }
        public int customerId { get; set; } 

        public DetailModel(IConfiguration configuration)
        {
            totals = new List<decimal>();   
            _httpClient = new HttpClient();
            _configuration = configuration;
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _httpClient.BaseAddress = new Uri(apiUrl);
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var log = HttpContext.Session.GetString("adminLogged");
            if (log == "logged")
            {
                var token = HttpContext.Session.GetString("adminToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/learner/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Learner = JsonConvert.DeserializeObject<LearnerBaseViewModel>(data);
                }
                HttpResponseMessage responseỎder = _httpClient.GetAsync(_httpClient.BaseAddress + "/order/history-of-learner?learnerId=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = responseỎder.Content.ReadAsStringAsync().Result;
                    orders = JsonConvert.DeserializeObject<List<OrderViewModel>>(data);
                    if (orders != null && orders.Count > 0)
                    {
                        await getTotal();
                    }
                }
                customerId = id;
                return Page();
            }
            else
            {
                TempData["error"] = "Please login with admin role to access this url!";
                return RedirectToPage("/Admin/Authentication/Login");
            }
        }

        public async Task getTotal()
        {
            foreach(var order in orders)
            {
                decimal total = 0;
                foreach(var item in order.Details)
                {
                    total += item.Price.GetValueOrDefault();
                }
                totals.Add(total);
            }
        }
    }
}
