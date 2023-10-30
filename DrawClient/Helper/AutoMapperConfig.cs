using AutoMapper;
using ViewModel.Course;

namespace DrawClient.Helper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CourseViewModel, CourseUpdateViewModel>()
               .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.Topic.Id));
        }
    }
}
