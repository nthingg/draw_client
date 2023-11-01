using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Base
{
	public class RegisterViewModel
	{
		[Required]
		[StringLength(30, ErrorMessage = "Name must be between 10 and 30 characters!", MinimumLength = 10)]
		public string Name { get; set; }

		[Required]
		[EmailAddress(ErrorMessage = "Invalid email address!")]
		public string Email { get; set; }

		[Required]
        [StringLength(50, ErrorMessage = "Password must be between 6 and 50 characters!", MinimumLength = 6)]
        public string Password { get; set; }	
	}
}
