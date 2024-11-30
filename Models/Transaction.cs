using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryAppMVC.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime TransactionDate { get; set; }
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
