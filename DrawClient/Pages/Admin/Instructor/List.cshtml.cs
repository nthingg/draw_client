using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Base;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class ListModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public Page<InstructorBaseViewModel> Instructors { get; set; }
        public ListModel()
        {
            _httpClient = new HttpClient();
            Instructors = new Page<InstructorBaseViewModel>();
        }
        public async Task<IActionResult> OnGetAsync(int pageIndex = 0)
        {
			var token = HttpContext.Session.GetString("adminToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = _httpClient.GetAsync(baseAddress+ "instructor/page?pageIndex="+pageIndex+"&pageSize=10").Result;
            if(response.IsSuccessStatusCode)
            {
                string data =  response.Content.ReadAsStringAsync().Result;
                Instructors = JsonConvert.DeserializeObject<Page<InstructorBaseViewModel>>(data);
            }
            return Page();
        }
    }
}
