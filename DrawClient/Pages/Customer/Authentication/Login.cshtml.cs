using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using ViewModel.Base;

namespace DrawClient.Pages.Customer.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        [BindProperty]
        public LoginViewModel Login { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataStr = JsonConvert.SerializeObject(Login);
                var content = new StringContent(dataStr, System.Text.Encoding.UTF8, "application/json");
                var res = await _client.PostAsync(_client.BaseAddress + "/learner/authorize", content);
                if (res.IsSuccessStatusCode)
                {
                    var resDataStr = await res.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<AuthorizedViewModel>(resDataStr);

                    var decodedToken = new JwtSecurityToken(data.AccessToken);
                    var id = decodedToken.Claims.First(c => c.Type == "RoleId").Value;

                    HttpContext.Session.SetString("learnerToken", data.AccessToken);
                    HttpContext.Session.SetString("learnerLogged", "logged");
                    HttpContext.Session.SetInt32("learnerId", int.Parse(id));
                    return Redirect("/Index");
                }
                else if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "Incorrect email or password");
                    return Page();
                }
            }
            return Page();
        }
    }
}
