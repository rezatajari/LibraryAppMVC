using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        Task Add(int userId, BookViewModel model);
        Task Remove( string bookTitle);
        Task<List<Book>> GetAll();
        Task<List<Book>> SearchByTitle(string title);
    }
}
