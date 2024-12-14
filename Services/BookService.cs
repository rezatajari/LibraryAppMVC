using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Validators;
using LibraryAppMVC.ViewModels;

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
        public async Task Add(int userId, BookViewModel model)
        {
            var newBook = new Book()
            {
                Title = model.Title,
                Author = model.Author,
                Genre = model.Genre
            };

            bool existBook = await _bookValidator.ExistValidation(newBook);

            if (existBook)
            {
                throw new ArgumentException("A book with the same title and author already exists.");
            }

            await _bookRepository.Add(newBook);

            int bookId = await _bookRepository.GetBookIdByTitle(newBook.Title);

            var tx = new Transaction()
            {
                UserId = userId,
                BookId = bookId,
                TransactionDate = DateTime.UtcNow

            };

            await _bookRepository.AddTransaction(tx);

        }

        public async Task Remove(Book book)
        {
            await _bookRepository.Remove(book);
        }

        public async Task<List<Book>> GetAll(int? userId)
        {
            var books = await _bookRepository.GetAll(userId);
            return books;
        }

        public async Task<Book> SearchByTitle(string title)
        {
            return await _bookRepository.SearchByTitle(title);
        }

    }
}
