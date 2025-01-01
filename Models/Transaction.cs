using System.ComponentModel.DataAnnotations;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryAppMVC.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int BookId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
