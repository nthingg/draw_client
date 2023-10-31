namespace Domain.Models
{
#pragma warning disable CS8618
    public class ExamResult
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int LearnerId { get; set; }
        public string AnswerUrl { get; set; }
        public double? Result { get; set; }
        public string? Comment { get; set; }
        //
        public Lesson Lesson { get; set; }
        public Learner Learner { get; set; }
    }
}
