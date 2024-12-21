using LibraryAppMVC.Models;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookRepository
    {
        Task Add(Book book);
        Task Remove(Book book);
        Task<List<Book>> GetAll(int? userId);
        Task<Book> SearchByTitle(string title,int? userId);
        Task<bool> ExistValidation(Book book,int? userId);
        Task<int> GetBookIdByTitle(string title);
        Task AddTransaction(Transaction tx);
    }
}
