using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryAppMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("Username")]
        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "The Confirm Password is not math")]
        public required string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Birthday Day")]
        public DateTime BirthdayDate { get; set; }
    }
}
