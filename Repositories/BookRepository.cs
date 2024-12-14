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
        public async Task<List<Book>> GetAll(int? userId)
        {
            var bookList = _libraryDB.Transactions
                                    .Where(u => u.UserId == userId)
                                    .Select(b => b.Book)
                                    .ToList();

            return bookList;
        }

        public async Task<Book> SearchByTitle(string title)
        {
            var book = await _libraryDB.Books.FirstOrDefaultAsync(b => b.Title == title);
            return book;
        }

        public async Task<bool> ExistValidation(Book book)
        {
            return await _libraryDB.Books.AnyAsync(b => b.Title == book.Title && b.Author == book.Author);
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
