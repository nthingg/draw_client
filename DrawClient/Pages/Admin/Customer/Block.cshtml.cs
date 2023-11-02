using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Base;
using ViewModel.Learner;

namespace DrawClient.Pages.Admin.Customer
{
    public class BlockModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public LearnerBaseViewModel Learner { get; set; }
        [BindProperty]
        public LearnerUpdateViewModel UpdateViewModel { get; set; }
        public BlockModel(IConfiguration configuration)
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
                UpdateViewModel = new LearnerUpdateViewModel
                {
                    Information = new UserBaseViewModel
                    {
                        Email = Learner.Information.Email,
                        Phone = Learner.Information.Phone,
                        Name = Learner.Information.Name,
                    }
                };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			var token = HttpContext.Session.GetString("adminToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/learner/status/" + Learner.Id).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/Customer/List");
            }
            return Page();
        }
    }
}
