
using System.ComponentModel.DataAnnotations;
using ViewModel.Base;

namespace ViewModel.Instructor
{
    public class InstructorUpdateViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage ="Description must be from 2 to 200 characters!", MinimumLength =2)]
        public string? Description { get; set; }
        public UserBaseViewModel Information { get; set; }
    }
}
