namespace ViewModel.Course
{
    public class CourseUpdateViewModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ThumbUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Level { get; set; }
        public int Material { get; set; }
        public int TopicId { get; set; }
    }
}
