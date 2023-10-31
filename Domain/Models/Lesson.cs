namespace Domain.Models
{
#pragma warning disable CS8618
    public class Lesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string? VideoUrl { get; set; }
        public string Content { get; set; }
        public bool IsExam { get; set; }
        //
        public Course Course { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }
    }
}
