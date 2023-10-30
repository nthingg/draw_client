using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class ListModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public ICollection<InstructorBaseViewModel> Instructors { get; set; }
        public ListModel()
        {
            _httpClient = new HttpClient();
            Instructors = new List<InstructorBaseViewModel>();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            HttpResponseMessage response = _httpClient.GetAsync(baseAddress+ "instructor").Result;
            if(response.IsSuccessStatusCode)
            {
                string data =  response.Content.ReadAsStringAsync().Result;
                Instructors = JsonConvert.DeserializeObject<List<InstructorBaseViewModel>>(data);
            }
            return Page();
        }
    }
}
