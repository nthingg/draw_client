using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Instructor;
using ViewModel.Learner;

namespace DrawClient.Pages.Admin.Customer
{
    public class DetailModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public LearnerBaseViewModel Learner { get; set; }   

        public DetailModel(IConfiguration configuration)
        {
            
            _httpClient = new HttpClient();
            _configuration = configuration;
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _httpClient.BaseAddress = new Uri(apiUrl);
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
			var token = HttpContext.Session.GetString("adminToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/learner/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                Learner = JsonConvert.DeserializeObject<LearnerBaseViewModel>(data);
            }
            return Page();
        }
    }
}
