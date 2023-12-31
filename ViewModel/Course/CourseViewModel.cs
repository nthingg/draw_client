﻿using ViewModel.Instructor;
using ViewModel.Lesson;
using ViewModel.Review;
using ViewModel.Topic;

namespace ViewModel.Course
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Level { get; set; }
        public string Material { get; set; }
        public bool IsAvailable { get; set; }
        public TopicBaseViewModel Topic { get; set; }
        public InstructorBaseViewModel Instructor { get; set; }
        public ICollection<LessonViewModel> Lessons { get; set; }
        public ICollection<ReviewViewModel> Reviews { get; set; }
    }
}
