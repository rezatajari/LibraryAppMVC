using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace LibraryAppMVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
        public string Author { get; set; }
        [Required]
        [EnumDataType(typeof(GenreType))]
        public GenreType Genre { get; set; }

        public enum GenreType
        {

            Fantasy = 1,
            ScienceFiction = 2,
            Biography = 3
        }

        public ICollection<Transaction> Transaction { get; set; }
    }
}
