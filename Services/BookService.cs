using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;
        public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }
        public async Task<ResultTask<bool>> Add(BookViewModel model, string userId)
        {
            // Check book validation
            var bookExist = await BookValidationExist(model, userId);
            if (!bookExist.Succeeded)
                return ResultTask<bool>.Failure("This book already exist in your account");

            // Create new book assign
            var newBook = new Book
            {
                Title = model.Title,
                Author = model.Author,
                Genre = model.Genre,
                UserId = userId
            };

            // Add to database
            var result = await _bookRepository.Add(newBook);

            // Check response to add
            if (!result.Succeeded)
            {
                _logger.LogError(result.ErrorMessage);
                return ResultTask<bool>.Failure("Error occurred while adding the book");
            }

            _logger.LogInformation("Book {newBook.Title} added successfully.", newBook.Title);
            return ResultTask<bool>.Success(true);

        }
        public async Task<ResultTask<bool>> Remove(BookViewModel model, string userId)
        {
            // Check book exist or not
            var checkBookExist = await BookValidationExist(model, userId);
            if (!checkBookExist.Succeeded)
                return ResultTask<bool>.Failure("This book is not exist");

            // Get book by title
            var book = await GetBookByTitle(model.Title, userId);

            // Remove operation
            var result = await _bookRepository.Remove(book.Data);
            if (!result.Succeeded)
            {
                _logger.LogError(result.ErrorMessage);
                return ResultTask<bool>.Failure("The remove is failed");
            }

            _logger.LogInformation("Book {model.Title} removed successfully.", model.Title);
            return ResultTask<bool>.Success(true);
        }
        public async Task<ResultTask<ListBookViewModel>> GetAll(string userId)
        {
            var result = await _bookRepository.GetAll(userId);
            if (!result.Succeeded)
            {
                _logger.LogError(result.ErrorMessage);
                return ResultTask<ListBookViewModel>.Failure("The list of book is not found");
            }

            var listModel = new ListBookViewModel
            {
                bookListViewMode = result.Data.Select(book => new BookViewModel
                {
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre
                }).ToList()
            };

            return ResultTask<ListBookViewModel>.Success(data: listModel);
        }
        public async Task<ResultTask<BookViewModel>> SearchByTitle(string title, string userId)
        {
            // Get book by title & validation it
            var book = await _bookRepository.SearchBookByTitle(title, userId);
            if (!book.Succeeded)
            {
                _logger.LogError(book.ErrorMessage);
                return ResultTask<BookViewModel>.Failure(book.ErrorMessage);
            }

            // Generate book model for view
            var bookModel = new BookViewModel
            {
                Title = book.Data.Title,
                Author = book.Data.Author,
                Genre = book.Data.Genre
            };

            _logger.LogInformation("Book {title} is found", title);
            return ResultTask<BookViewModel>.Success(bookModel);
        }
        public async Task<ResultTask<bool>> Delete(string userId, string title)
        {
            try
            {
               await _bookRepository.Delete(userId, title);
                _logger.LogInformation("Book {title} is deleted");
                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResultTask<bool>.Failure(ex.Message);
            }
        }

        private async Task<ResultTask<bool>> BookValidationExist(BookViewModel newBook, string userId)
        {
            var result = await _bookRepository.ExistValidation(newBook, userId);

            if (result.Succeeded) return ResultTask<bool>.Success(true);

            _logger.LogError(result.ErrorMessage);
            return ResultTask<bool>.Failure(result.ErrorMessage);

        }
        private async Task<ResultTask<Book>> GetBookByTitle(string title, string userId)
        {
            var result = await _bookRepository.GetBookByTitle(title, userId);

            if (!result.Succeeded)
            {
                _logger.LogError(result.ErrorMessage);
                return ResultTask<Book>.Failure(result.ErrorMessage);
            }

            _logger.LogInformation("Book {title} is founded", title);
            return ResultTask<Book>.Success(data: result.Data);
        }
    }
}
