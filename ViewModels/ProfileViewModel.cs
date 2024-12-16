using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryAppMVC.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public string? ExistProfilePicture { get; set; }
    }
}
