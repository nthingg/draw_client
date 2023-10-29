using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Customer
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
        public void OnGet()
        {
        }
    }
}
