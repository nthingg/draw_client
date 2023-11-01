using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Win32;
using Newtonsoft.Json;
using ViewModel.Base;

namespace DrawClient.Pages.Admin.Customer
{
    public class CreateModel : PageModel
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public SignUpViewModel learner { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; } 

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _httpClient.BaseAddress = new Uri(apiUrl);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(learner.Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and confirm password must be the same!");
                return Page();
            }
            if (ModelState.IsValid)
            {
                var dataStr = JsonConvert.SerializeObject(learner);
                var content = new StringContent(dataStr, System.Text.Encoding.UTF8, "application/json");
                var res = await _httpClient.PostAsync(_httpClient.BaseAddress + "/learner/register", content);
                if (res.IsSuccessStatusCode)
                {
                    return Redirect("/Admin/Customer/List");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Register fail");
                    return Page();
                }
            }
            return Page();
        }
    }
}
