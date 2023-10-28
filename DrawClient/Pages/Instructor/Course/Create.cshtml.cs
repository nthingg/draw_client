using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection.Emit;
using ViewModel.Base;
using ViewModel.Course;
using ViewModel.Lesson;
using ViewModel.Topic;

namespace DrawClient.Pages.Instructor.Course
{
    public class CreateModel : PageModel
    {
		private readonly IConfiguration _configuration;
		private readonly HttpClient _client;

		public CreateModel(IConfiguration configuration)
		{
			_configuration = configuration;
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
