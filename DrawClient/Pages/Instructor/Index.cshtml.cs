using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using ViewModel.Base;

namespace DrawClient.Pages.Instructor
{
    public class IndexModel : PageModel
    {
		private readonly IConfiguration _configuration;
		private readonly HttpClient _client;

		public IndexModel(IConfiguration configuration)
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
			var dataStr = JsonConvert.SerializeObject(Login);
			var content = new StringContent(dataStr, System.Text.Encoding.UTF8, "application/json");
			var res = await _client.PostAsync(_client.BaseAddress + "/instructor/authorize", content);
			if (res.IsSuccessStatusCode)
			{
				var resDataStr = await res.Content.ReadAsStringAsync();
				var data = JsonConvert.DeserializeObject<AuthorizedViewModel>(resDataStr);
				HttpContext.Session.SetString("instructToken", data.AccessToken);
				HttpContext.Session.SetInt32("instructId", data.UserId);
				return Redirect("/Instructor/Course");
			}
			else if (res.StatusCode == HttpStatusCode.Unauthorized)
			{
				ModelState.AddModelError(string.Empty, "Incorrect email or password");
				return Page();
			}

			return Page();
		}

		public IActionResult OnGetLogout()
		{
			HttpContext.Session.Clear();
			return Redirect("/Instructor/Index");
		}
    }
}
