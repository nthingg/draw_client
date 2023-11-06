using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ViewModel.Base;
using ViewModel.Course;
using ViewModel.Instructor;

namespace DrawClient.Pages.Admin.Instructor
{
    public class DetailModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public InstructorViewModel instructor { get; set; }
        public Page<CourseViewModel> courses { get; set; }
        public DetailModel()
        {
            _httpClient = new HttpClient();
            courses = new Page<CourseViewModel>();
            courses.Items = new List<CourseViewModel>();    
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var log = HttpContext.Session.GetString("adminLogged");
            if (log == "logged")
            {
                var token = HttpContext.Session.GetString("adminToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = _httpClient.GetAsync(baseAddress + "instructor/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    instructor = JsonConvert.DeserializeObject<InstructorViewModel>(data);
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseCourse = _httpClient.GetAsync(baseAddress + $"course/by-instructor?instructorId={id}&pageIndex=0&pageSize=10").Result;
                if (responseCourse.IsSuccessStatusCode)
                {
                    string data = responseCourse.Content.ReadAsStringAsync().Result;
                    courses = JsonConvert.DeserializeObject<Page<CourseViewModel>>(data);
                }
                return Page();
            }
            else
            {
                TempData["error"] = "Please login with admin role to access this url!";
                return RedirectToPage("/Admin/Authentication/Login");
            }
            
        }
    }
}
