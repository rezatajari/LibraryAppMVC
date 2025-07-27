using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        Task<ResultTask<bool>> Add(BookViewModel model, string userId);
        Task<ResultTask<bool>> Remove(int bookId, string userId);
        Task<ResultTask<ListBookViewModel>> GetAll(string userId);
        Task<ResultTask<BookViewModel>> SearchByTitle(string title, string userId);
        Task<ResultTask<bool>> Delete(string userId, string title);
        Task<ResultTask<Book>> GetByIdAsync(int id);
        Task<ResultTask<bool>> UpdateAsync(Book book);
    }
}
