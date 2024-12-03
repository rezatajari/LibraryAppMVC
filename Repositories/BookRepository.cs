using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibraryAppMVC.Repositories
{
    public class BookRepository : IBookRepository
    {

        private readonly LibraryDB _libraryDB;
        public BookRepository(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }

        public void Add(Book book)
        {
            _libraryDB.Books.Add(book);
            _libraryDB.SaveChanges();
        }

        public void Remove(Book book)
        {
            _libraryDB.Books.Remove(book);
            _libraryDB.SaveChanges();
        }
        public List<Book> GetAll()
        {
            return _libraryDB.Books.ToList();
        }

        public List<Book> SearchByTitle(string title)
        {
            return _libraryDB.Books.Where(b => b.Title == title).ToList();
        }

        public bool ExistValidation(Book book)
        {
            return _libraryDB.Books.Any(b => b.Title == book.Title && b.Author == book.Author);
        }
    }
}
