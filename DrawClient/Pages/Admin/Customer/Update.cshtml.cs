using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using ViewModel.Base;
using ViewModel.Learner;

namespace DrawClient.Pages.Admin.Customer
{
    public class UpdateModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public LearnerBaseViewModel Learner { get; set; }
        [BindProperty]
        public LearnerUpdateViewModel UpdateViewModel { get; set; }
        public UpdateModel(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _httpClient.BaseAddress = new Uri(apiUrl);
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
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

            //LearnerUpdateViewModel model = new LearnerUpdateViewModel {
            //    Information = new UserBaseViewModel {
            //        Email = Learner.Information.Email,
            //        Phone = Learner.Information.Phone,
            //        Name = Learner.Information.Name,
            //    }
            //};
            string data = JsonConvert.SerializeObject(UpdateViewModel);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/learner/" + Learner.Id, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Learner information updated!";
                return RedirectToPage("/Admin/Customer/List");
            }
            return Page();
        }
    }
}
