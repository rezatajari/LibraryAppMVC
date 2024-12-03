using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace LibraryAppMVC.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100,ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string Username { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$", ErrorMessage = "Password" +
            " must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string Password { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

    }
}
