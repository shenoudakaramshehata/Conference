using System.ComponentModel.DataAnnotations;

namespace Conference.Models
{
    public class ChangePasswordVM
    {

		[DataType(DataType.EmailAddress)]
		public string? CurrentEmail { get; set; }
		[Required]
        [DataType(DataType.Password)]
        [Display(Name ="Current Password")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Should have at least one lower case , one upper case , one number,one special character and minimum length 6 characters")]
        public string CurrentPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Should have at least one lower case , one upper case,one number , one special character and minimum length 6 characters")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$",ErrorMessage = "Should have at least one lower case , one upper case , one number,one special character and minimum length 6 characters")]
        [Compare("NewPassword",ErrorMessage ="The New Password and Confirmation Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
