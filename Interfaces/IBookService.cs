using LibraryAppMVC.Models;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        void Add(Book newBook);
        void Remove(Book newBook);
        void Search();
    }
}
