namespace Domain.Models
{
#pragma warning disable CS8618
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //
        public ICollection<Course> Courses { get; set; }
    }
}
