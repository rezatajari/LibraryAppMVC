using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Services;

namespace LibraryAppMVC.Validators
{
    public class BookValidator
    {
        private readonly IBookRepository _bookRepository;
        public BookValidator(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public bool ExistValidation(Book book)
        {
            return _bookRepository.ExistValidation(book);
        }

    }
}
