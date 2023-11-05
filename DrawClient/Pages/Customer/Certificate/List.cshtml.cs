using DrawchadGRPCServer;
using DrawchadViewModel.Certificate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Course;

namespace DrawClient.Pages.Customer.Certificate
{
    public class ListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public ListModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        public Page<CertificateViewModel> Certificates { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string? learnerLogged = HttpContext.Session.GetString("learnerLogged");
            if (learnerLogged != "logged")
            {
                return Redirect("/Customer/Authentication/Login");
            }

            await Refresh();
            return Page();
        }

        private async Task Refresh(int pageIndex = 0)
        {
            var token = HttpContext.Session.GetString("learnerToken");
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/learner/certificates?pageIndex={pageIndex}&pageSize={8}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var certs = JsonConvert.DeserializeObject<Page<CertificateViewModel>>(dataStr);
                if (certs is not null)
                {
                    Certificates = certs;
                }
            }
        }
    }   
}
