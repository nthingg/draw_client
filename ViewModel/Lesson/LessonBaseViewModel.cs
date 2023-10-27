namespace ViewModel.Lesson
{
    public class LessonBaseViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsExam { get; set; }
    }
}
