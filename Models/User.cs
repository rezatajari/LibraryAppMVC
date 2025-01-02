using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Models
{
    public class User : IdentityUser
    {
        public string? ProfilePicturePath { get; set; }
        public DateTime BirthdayDate { get; set; }

    }
}
