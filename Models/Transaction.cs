using System.ComponentModel.DataAnnotations;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryAppMVC.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        [DataType(DataType.Date)]   
        public DateTime TransactionDate { get; set; }
        [EnumDataType(typeof(TransactionType))]
        public TransactionType Type { get; set; }

        public enum TransactionType
        {
            Borrow,
            Return
        }

        public Book Book { get; set; }
        public User User { get; set; }
    }
}
