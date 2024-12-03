using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Validators;

namespace LibraryAppMVC.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly BookValidator _bookValidator;
        public BookService(IBookRepository bookRepository,
            BookValidator bookValidation)
        {
            _bookRepository = bookRepository;
            _bookValidator = bookValidation;
        }
        public void Add(Book newBook)
        {
            if (_bookValidator.ExistValidation(newBook))
            {
                throw new ArgumentException("A book with the same title and author already exists.");
            }

            _bookRepository.Add(newBook);
        }

        public void Remove(Book newBook)
        {
            _bookRepository.Remove(newBook);
        }

        public List<Book> GetAll()
        {
            return _bookRepository.GetAll();
        }

        public List<Book> SearchByTitle(string title)
        {
            return _bookRepository.SearchByTitle(title);
        }

    }
}
