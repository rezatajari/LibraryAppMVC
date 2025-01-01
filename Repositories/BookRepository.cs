using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Repositories
{
    public class BookRepository : IBookRepository
    {

        private readonly LibraryDB _libraryDB;
        public BookRepository(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }

        public async Task<ResultTask<bool>> Add(Book book)
        {
            try
            {
                await _libraryDB.Books.AddAsync(book);
                await _libraryDB.SaveChangesAsync();

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
                _libraryDB.Books.Remove(book);
                await _libraryDB.SaveChangesAsync();
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
                var bookList = await _libraryDB.Books
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
                var book = await _libraryDB.Books
                    .Where(b => b.Title == title && b.UserId == userId)
                    .FirstOrDefaultAsync();

                return ResultTask<Book>.Success(data: book);
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
                await _libraryDB.Books
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
                var book = await _libraryDB.Books.FirstOrDefaultAsync(b => b.Title == title && b.UserId == userId);
                return ResultTask<Book>.Success(book);
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
                var book = await _libraryDB.Books.FirstOrDefaultAsync(b => b.Title == title && b.UserId == userId);
                _libraryDB.Books.Remove(book);
                await _libraryDB.SaveChangesAsync();
                return ResultTask<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ResultTask<bool>.Failure(ex.Message);
            }
        }
    }
}
