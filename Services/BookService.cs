using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibraryAppMVC.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDB _libraryDB;

        public BookService(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }
        public void Add(Book newBook)
        {
            try
            {
                _libraryDB.Books.Add(newBook);
            }
            catch (Exception err)
            {

               // return View(err.Message);
            }
        }

        public void Remove(Book newBook)
        {
        }

        public void Search()
        {
        }
    }
}
