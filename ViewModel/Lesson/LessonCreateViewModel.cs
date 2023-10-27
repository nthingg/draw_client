namespace ViewModel.Lesson
{
    public class LessonCreateViewModel
    {
        public string Title { get; set; }
        public string? VideoUrl { get; set; }
        public string Content { get; set; }
        public bool IsExam { get; set; }
    }
}
