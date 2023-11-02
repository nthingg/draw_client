using DrawchadGRPCServer;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ViewModel.Base;
using ViewModel.Course;
using ViewModel.Instructor;
using ViewModel.Lesson;
using ViewModel.Topic;
using static DrawchadGRPCServer.CourseSrv;

namespace DrawClient.Pages.Customer.Course
{
    public class PurchasedModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly string gRPCUrl;

        public PurchasedModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
            var apiUrl = _configuration.GetSection("ApiUrl").Get<string>();
            _client.BaseAddress = new Uri(apiUrl);
            gRPCUrl = _configuration.GetSection("gRPC").Get<string>();
        }

        public List<CourseViewModel> Courses { get; set; }

        public Page<CourseViewModel> CoursesReview { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await GetCoursesList();
            await Refresh();
            return Page();
        }

        private async Task GetCoursesList()
        {
            var token = HttpContext.Session.GetString("learnerToken");

            var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
            {
                metadata.Add("Authorization", $"Bearer {token}");
            });
                    
            using var channel = GrpcChannel.ForAddress(gRPCUrl, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            var client = new CourseSrvClient(channel);
            var courses = await client.GetPurchasedCourseAsync(new Empty { });
            var courseResult = new List<CourseViewModel>();
            foreach (var item in courses.Items)
            {
                var lessons = new List<LessonViewModel>();
                foreach (var lessonItem in item.Lessons.Items)
                {
                    var lesson = new LessonViewModel
                    {
                        Id = lessonItem.Id,
                        CourseId = lessonItem.CourseId,
                        Title = lessonItem.Title,
                        Content = lessonItem.Content,
                        VideoUrl = lessonItem.VideoUrl ?? "",
                        IsExam = lessonItem.IsExam,
                    };
                    lessons.Add(lesson);
                }

                var course = new CourseViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ThumbUrl = item.ThumbUrl,
                    OriginalPrice = (decimal)item.OriginalPrice,
                    DiscountPrice = (decimal)item.DiscountPrice,
                    Level = item.Level,
                    Material = item.Material,
                    Topic = new TopicBaseViewModel
                    {
                        Id = item.Topic.Id,
                        Name = item.Topic.Name,
                    },
                    Instructor = new InstructorBaseViewModel
                    {
                        Id = item.Instructor.Id,
                        Information = new UserBaseViewModel
                        {
                            Id = item.Instructor.User.Id,
                            Name = item.Instructor.User.Name,
                            Email = item.Instructor.User.Email,
                            Phone = item.Instructor.User.Phone ?? "",
                            IsBlocked = item.Instructor.User.IsBlocked
                        }
                    },
                    Lessons = lessons
                };
                courseResult.Add(course);
            }

            Courses = courseResult;
        }

        private async Task Refresh()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/course/page?pageIndex=0&pageSize=20");
            var res = await _client.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                var dataStr = await res.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<Page<CourseViewModel>>(dataStr);
                if (courses is not null)
                {
                    CoursesReview = courses;
                }
            }
        }
    }
}
