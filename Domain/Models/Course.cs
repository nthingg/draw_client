using Domain.Enums;

namespace Domain.Models
{
#pragma warning disable CS8618
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ThumbUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public CourseLevel Level { get; set; }
        public CourseMaterial Material { get; set; }
        public bool IsAvailable { get; set; }
        //
        public int InstructorId { get; set; }
        public int TopicId { get; set; }
        //
        public Instructor Instructor { get; set; }
        public Topic Topic { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
