using LibraryAppMVC.Models;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookRepository
    {
        Task Add(Book book);
        Task Remove(Book book);
        Task<List<Book>> GetAll();
        Task<List<Book>> SearchByTitle(string title);
        Task<bool> ExistValidation(Book book);
        Task<int> GetBookIdByTitle(string title);
        Task<Book> GetBookByTitle(string title);
        Task AddTransaction(Transaction tx);
    }
}
