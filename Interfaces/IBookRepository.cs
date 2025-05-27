using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookRepository
    {
        Task<ResultTask<bool>> Add(Book book);
        Task<ResultTask<bool>> Remove(Book book);
        Task<ResultTask<List<Book>>> GetAll(string userId);
        Task<ResultTask<Book>> GetBookByTitle(string title, string userId);
        Task<ResultTask<bool>> ExistValidation(BookViewModel book, string userId);
        Task<ResultTask<Book>> SearchBookByTitle(string title, string userId);
        Task<ResultTask<bool>> Delete(string userId, string title);
    }
}
