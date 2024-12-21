using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace LibraryAppMVC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string Email { get; set; }
        public DateTime BirthdayDate { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

    }
}
