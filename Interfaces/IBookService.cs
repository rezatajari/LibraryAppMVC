using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        Task<ResultTask<bool>> Add(BookViewModel model, string userId);
        Task<ResultTask<bool>> Remove(BookViewModel model, string userId);
        Task<ResultTask<ListBookViewModel>> GetAll(string userId);
        Task<ResultTask<BookViewModel>> SearchByTitle(string title, string userId);
        Task<ResultTask<bool>> Delete(string userId, string title);
    }
}
