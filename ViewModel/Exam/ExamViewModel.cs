using ViewModel.Learner;
using ViewModel.Lesson;

namespace ViewModel.Exam
{
    public class ExamViewModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string AnswerUrl { get; set; }
        public double? Result { get; set; }
        public bool IsPassed { get; set; }
        public string? Comment { get; set; }
        public LearnerBaseViewModel Learner { get; set; }
        public LessonViewModel Lesson { get; set; }
        public string CourseName { get; set; }
        public string ThumbUrl { get; set; }
    }
}
