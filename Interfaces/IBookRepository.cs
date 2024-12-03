using LibraryAppMVC.Models;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookRepository
    {
        void Add(Book book);
        void Remove(Book book);
        List<Book> GetAll();
        List<Book> SearchByTitle(string title);
        bool ExistValidation(Book book);
    }
}
