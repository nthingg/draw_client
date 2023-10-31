﻿using DrawchadViewModel.Instructor;
using DrawchadViewModel.Lesson;
using DrawchadViewModel.Topic;

namespace DrawchadViewModel.Course
{
    public class PurchasedCourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public string Level { get; set; }
        public string Material { get; set; }
        public TopicBaseViewModel Topic { get; set; }
        public InstructorBaseViewModel Instructor { get; set; }
        public ICollection<LessonViewModel> Lessons { get; set; }
    }
}