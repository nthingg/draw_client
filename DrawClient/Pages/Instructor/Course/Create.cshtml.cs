using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using ViewModel.Course;
using ViewModel.Topic;

namespace DrawClient.Pages.Instructor.Course
{
    public class CreateModel : PageModel
    {
		private readonly IConfiguration _configuration;
		private readonly HttpClient _client;
		private readonly IWebHostEnvironment _environment;

        public CreateModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
        }

        [BindProperty]
		public CourseCreateViewModel Course { get; set; }
		[BindProperty]
		public IFormFile Image { get; set; }
		public SelectList Topic { get; set; }
		public SelectList Material { get; set; }
		public SelectList Level { get; set; }

		public async Task OnGetAsync()
        {
			await GenerateLevelOptions();
			await GenerateMaterialOptions();
			await GenerateTopicOptions();
        }

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
            {
                // Get the full path to the wwwroot directory.
                string wwwrootPath = _environment.WebRootPath;

                Guid name = Guid.NewGuid();
                var filePath = Path.Combine(wwwrootPath, "images", "course", "imgs", name.ToString() + Path.GetExtension(Image.FileName));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

				Course.ThumbUrl = @"/images/course/imgs/" + name.ToString() + Path.GetExtension(Image.FileName);

				var dataStr = JsonConvert.SerializeObject(Course);
				var content = new StringContent(dataStr, Encoding.UTF8, "application/json");
				var token = HttpContext.Session.GetString("instructToken");
				var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/course");
				request.Headers.Add("Authorization", $"Bearer {token}");
				request.Content = content;

				//
				var res = await _client.SendAsync(request);
				if (res.IsSuccessStatusCode)
				{
					return RedirectToPage("/Instructor/Course/AddLesson", Course.Name);
				}
            }

            await GenerateLevelOptions();
            await GenerateMaterialOptions();
            await GenerateTopicOptions();

            return Page();
		}

		private async Task GenerateTopicOptions()
		{
			var res = await _client.GetAsync(_client.BaseAddress + "/base/topic");
			if (res.IsSuccessStatusCode)
			{
				var dataStr = await res.Content.ReadAsStringAsync();
				var topics = JsonConvert.DeserializeObject<List<TopicBaseViewModel>>(dataStr);
				Topic = new SelectList(topics, nameof(TopicBaseViewModel.Id), nameof(TopicBaseViewModel.Name));
			}
		}

		private async Task GenerateLevelOptions()
		{
			var res = await _client.GetAsync(_client.BaseAddress + "/base/level");
			if (res.IsSuccessStatusCode)
			{
				var dataStr = await res.Content.ReadAsStringAsync();
				var level = JsonConvert.DeserializeObject<List<TopicBaseViewModel>>(dataStr);
				Level = new SelectList(level, nameof(TopicBaseViewModel.Id), nameof(TopicBaseViewModel.Name));
			}
		}

		private async Task GenerateMaterialOptions()
		{
			var res = await _client.GetAsync(_client.BaseAddress + "/base/material");
			if (res.IsSuccessStatusCode)
			{
				var dataStr = await res.Content.ReadAsStringAsync();
				var materials = JsonConvert.DeserializeObject<List<TopicBaseViewModel>>(dataStr);
				Material = new SelectList(materials, nameof(TopicBaseViewModel.Id), nameof(TopicBaseViewModel.Name));
			}
		}
	}
}
