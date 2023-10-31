using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using ViewModel.Base;

namespace DrawClient.Pages.Customer.Authentication
{
    public class RegisterModel : PageModel
    {
		private readonly IConfiguration _configuration;
		private readonly HttpClient _client;

		public RegisterModel(IConfiguration configuration)
		{
			_configuration = configuration;
			_client = new HttpClient();
			var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
			_client.BaseAddress = new Uri(apiUrl);
		}

		[BindProperty]
		public RegisterViewModel Register { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var dataStr = JsonConvert.SerializeObject(Register);
				var content = new StringContent(dataStr, System.Text.Encoding.UTF8, "application/json");
				var res = await _client.PostAsync(_client.BaseAddress + "/learner/register", content);
				if (res.IsSuccessStatusCode)
				{
					return Redirect("/Customer/Authentication/Login");
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
