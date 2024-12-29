using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task Add(Book book)
        {
            await _libraryDB.Books.AddAsync(book);
            await _libraryDB.SaveChangesAsync();
        }

        public async Task Remove(Book book)
        {
            _libraryDB.Books.Remove(book);
            await _libraryDB.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAll(string userId)
        {
            var bookList = await _libraryDB.Transactions
                                    .Where(u => u.UserId == userId)
                                    .Select(b => b.Book)
                                    .ToListAsync();

            return bookList;
        }

        public async Task<Book> SearchByTitle(string title, string userId)
        {
            var book = await _libraryDB.Transactions
                                        .Where(b => b.Book.Title == title && b.UserId == userId)
                                        .Select(b => b.Book)
                                        .FirstOrDefaultAsync();
            return book;
        }

        public async Task<bool> ExistValidation(Book book, string userId)
        {
            return await _libraryDB.Transactions
                                   .Where(b => b.Book.Title == book.Title && b.Book.Author == book.Author && b.UserId == userId)
                                   .AnyAsync();
        }

        public async Task<int> GetBookIdByTitle(string title)
        {
            return await _libraryDB.Books
                                   .Where(b => b.Title == title)
                                   .Select(b => b.Id)
                                   .FirstOrDefaultAsync();
        }

        public async Task AddTransaction(Transaction tx)
        {
            await _libraryDB.Transactions.AddAsync(tx);
            await _libraryDB.SaveChangesAsync();
        }
    }
}
