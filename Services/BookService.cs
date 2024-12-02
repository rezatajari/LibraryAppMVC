using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;

namespace LibraryAppMVC.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDB _libraryDB;

        public BookService(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }
        public void Add()
        {

        }

        public void Remove()
        {
        }

        public void Search()
        {
        }
    }
}
