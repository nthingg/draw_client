using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Instructor;
using ViewModel.Learner;

namespace DrawClient.Pages.Admin.Customer
{
    public class ListModel : PageModel
    {
        Uri baseAddress = new Uri("http://localhost:5173/api/");
        private HttpClient _httpClient;
        public ICollection<LearnerBaseViewModel> Learners { get; set; }
        public Page<LearnerBaseViewModel> page { get; set; }
        public ListModel()
        {
            _httpClient = new HttpClient();
            Learners = new List<LearnerBaseViewModel>();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            HttpResponseMessage response = _httpClient.GetAsync(baseAddress + "learner/page?pageIndex=0&pageSize=10").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var rs = JsonConvert.DeserializeObject<Page<LearnerBaseViewModel>>(data);
                Learners = rs.Items;
            }
            return Page();
        }
    }
}
