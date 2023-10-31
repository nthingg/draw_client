namespace Domain.Models
{
#pragma warning disable CS8618
    public class Instructor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Description { get; set; }
        public User User { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
