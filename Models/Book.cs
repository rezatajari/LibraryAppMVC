using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace LibraryAppMVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public GenreType Genre { get; set; }
        public ICollection<Transaction> Transaction { get; set; }

        public enum GenreType
        {

            Fantasy = 1,
            ScienceFiction = 2,
            Biography = 3
        }

    }
}
