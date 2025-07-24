using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryAppMVC.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [DisplayName("Username")]
        public string? UserName { get; set; }

        public IFormFile? ProfilePicture { get; set; }
        public DateTime BirthdayDate { get; set; }
        public string? ExistProfilePicture { get; set; }
    }
}
