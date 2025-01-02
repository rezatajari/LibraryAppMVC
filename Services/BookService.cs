using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Services
{
    public class BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        : IBookService
    {
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
            var result = await bookRepository.Add(newBook);

            // Check response to add
            if (!result.Succeeded)
            {
                logger.LogError(result.ErrorMessage);
                return ResultTask<bool>.Failure("Error occurred while adding the book");
            }

            logger.LogInformation("Book {newBook.Title} added successfully.", newBook.Title);
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
            if (book.Data != null)
            {
                var result = await bookRepository.Remove(book.Data);
                if (!result.Succeeded)
                {
                    logger.LogError(result.ErrorMessage);
                    return ResultTask<bool>.Failure("The remove is failed");
                }
            }

            logger.LogInformation("Book {model.Title} removed successfully.", model.Title);
            return ResultTask<bool>.Success(true);
        }
        public async Task<ResultTask<ListBookViewModel>> GetAll(string userId)
        {
            var result = await bookRepository.GetAll(userId);
            if (!result.Succeeded)
            {
                logger.LogError(result.ErrorMessage);
                return ResultTask<ListBookViewModel>.Failure("The list of book is not found");
            }

            if (result.Data != null)
            {
                var listModel = new ListBookViewModel
                {
                    BookListViewMode = result.Data.Select(book => new BookViewModel
                    {
                        Title = book.Title,
                        Author = book.Author,
                        Genre = book.Genre
                    }).ToList()
                };

                return ResultTask<ListBookViewModel>.Success(data: listModel);
            }

            logger.LogError(result.ErrorMessage);
            return ResultTask<ListBookViewModel>.Failure("Get list has error");
        }
        public async Task<ResultTask<BookViewModel>> SearchByTitle(string title, string userId)
        {
            // Get book by title & validation it
            var book = await bookRepository.SearchBookByTitle(title, userId);
            if (!book.Succeeded)
            {
                logger.LogError(book.ErrorMessage);
                return ResultTask<BookViewModel>.Failure(book.ErrorMessage);
            }

            // Generate book model for view
            if (book.Data == null) 
                return ResultTask<BookViewModel>.Failure("The book is null");
            var bookModel = new BookViewModel
            {
                Title = book.Data.Title,
                Author = book.Data.Author,
                Genre = book.Data.Genre
            };

            logger.LogInformation("Book {title} is found", title);
            return ResultTask<BookViewModel>.Success(bookModel);
        }
        public async Task<ResultTask<bool>> Delete(string userId, string title)
        {
            try
            {
                await bookRepository.Delete(userId, title);
                logger.LogInformation("Book {title} is deleted", title);
                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return ResultTask<bool>.Failure(ex.Message);
            }
        }

        private async Task<ResultTask<bool>> BookValidationExist(BookViewModel newBook, string userId)
        {
            var result = await bookRepository.ExistValidation(newBook, userId);

            if (result.Succeeded) return ResultTask<bool>.Success(true);

            logger.LogError(result.ErrorMessage);
            return ResultTask<bool>.Failure(result.ErrorMessage);

        }
        private async Task<ResultTask<Book>> GetBookByTitle(string? title, string userId)
        {
            // Check title is exist or not
            if (string.IsNullOrEmpty(title))
                return ResultTask<Book>.Failure("Title is null or empty");

            var result = await bookRepository.GetBookByTitle(title, userId);

            if (!result.Succeeded)
            {
                logger.LogError(result.ErrorMessage);
                return ResultTask<Book>.Failure(result.ErrorMessage);
            }

            logger.LogInformation("Book {title} is founded", title);
            if (result.Data != null) return ResultTask<Book>.Success(data: result.Data);
            return ResultTask<Book>.Failure("Book is null");
        }
    }
}
