using ViewModel.Base;

namespace ViewModel.Instructor
{
    public class InstructorViewModel
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public UserBaseViewModel Information { get; set; }
    }
}
