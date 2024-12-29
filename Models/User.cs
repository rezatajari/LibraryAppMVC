using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Models
{
    public class User : IdentityUser
    {
        public string? ProfilePicturePath { get; set; }
        public DateTime BirthdayDate { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

    }
}
