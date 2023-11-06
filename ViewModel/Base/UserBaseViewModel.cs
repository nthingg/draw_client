using System.ComponentModel.DataAnnotations;

namespace ViewModel.Base
{
    public class UserBaseViewModel
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(30, ErrorMessage ="Name must be from 2 to 30 characters!", MinimumLength =2)]
        public string Name { get; set; }
        [StringLength(10, ErrorMessage ="Phone must be from 10 to 11 characters!", MinimumLength =10)]
        public string? Phone { get; set; }
        public bool IsBlocked { get; set; }
    }
}
