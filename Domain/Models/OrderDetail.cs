namespace Domain.Models
{
#pragma warning disable CS8618
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CourseId { get; set; }
        public decimal? Price { get; set; }
        public string? Comment { get; set; }
        public double? Rating { get; set; }
        //
        public Order Order { get; set; }
        public Course Course { get; set; }
    }
}
