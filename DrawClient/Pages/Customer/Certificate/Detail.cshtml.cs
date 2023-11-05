using DrawchadViewModel.Certificate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Course;
using ViewModel.Learner;

namespace DrawClient.Pages.Customer.Certificate
{
    public class DetailModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public DetailModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public CertificateViewModel Certificate { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            string? learnerLogged = HttpContext.Session.GetString("learnerLogged");
            if (learnerLogged != "logged")
            {
                return Redirect("/Customer/Authentication/Login");
            }

            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/learner/certificates/" + id);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var cert = JsonConvert.DeserializeObject<CertificateViewModel>(dataStr);
                Certificate = cert;
            }
            return Page();
        }
    }
}
