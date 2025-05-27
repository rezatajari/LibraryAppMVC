using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using Microsoft.EntityFrameworkCore;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Repositories
{
    public class BookRepository(LibraryDb libraryDb) : IBookRepository
    {
        public async Task<ResultTask<bool>> Add(Book book)
        {
            try
            {
                await libraryDb.Books.AddAsync(book);
                await libraryDb.SaveChangesAsync();

                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ResultTask<bool>.Failure(ex.Message);
            }
        }

        public async Task<ResultTask<bool>> Remove(Book book)
        {
            try
            {
                libraryDb.Books.Remove(book);
                await libraryDb.SaveChangesAsync();
                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ResultTask<bool>.Failure(ex.Message);
            }
        }

        public async Task<ResultTask<List<Book>>> GetAll(string userId)
        {
            try
            {
                var bookList = await libraryDb.Books
                    .Where(u => u.UserId == userId)
                    .ToListAsync();

                return ResultTask<List<Book>>.Success(data: bookList);
            }
            catch (Exception ex)
            {
                return ResultTask<List<Book>>.Failure(ex.Message);
            }
        }

        public async Task<ResultTask<Book>> GetBookByTitle(string title, string userId)
        {
            try
            {
                var book = await libraryDb.Books
                    .Where(b => b.Title == title && b.UserId == userId)
                    .FirstOrDefaultAsync();

                return book != null ? ResultTask<Book>.Success(data: book) : ResultTask<Book>.Failure("Something wrong");
            }
            catch (Exception ex)
            {
                return ResultTask<Book>.Failure(ex.Message);
            }
        }

        public async Task<ResultTask<bool>> ExistValidation(BookViewModel book, string userId)
        {
            try
            {
                await libraryDb.Books
                    .Where(b => b.Title == book.Title && b.Author == book.Author && b.UserId == userId)
                    .AnyAsync();

                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ResultTask<bool>.Failure(ex.Message);
            }
        }

        public async Task<ResultTask<Book>> SearchBookByTitle(string title, string userId)
        {
            try
            {
                var book = await libraryDb.Books.FirstOrDefaultAsync(b => b.Title == title && b.UserId == userId);
                if (book != null) return ResultTask<Book>.Success(book);
                return ResultTask<Book>.Failure("Book is null");
            }
            catch (Exception ex)
            {
                return ResultTask<Book>.Failure(ex.Message);
            }
        }

        public async Task<ResultTask<bool>> Delete(string userId, string title)
        {
            try
            {
                var book = await libraryDb.Books.FirstOrDefaultAsync(b => b.Title == title && b.UserId == userId);
                if (book != null) libraryDb.Books.Remove(book);
                await libraryDb.SaveChangesAsync();
                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ResultTask<bool>.Failure(ex.Message);
            }
        }
    }
}
