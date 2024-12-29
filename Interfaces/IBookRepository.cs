using LibraryAppMVC.Models;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookRepository
    {
        Task Add(Book book);
        Task Remove(Book book);
        Task<List<Book>> GetAll(string userId);
        Task<Book> SearchByTitle(string title,string userId);
        Task<bool> ExistValidation(Book book,string userId);
        Task<int> GetBookIdByTitle(string title);
        Task AddTransaction(Transaction tx);
    }
}
