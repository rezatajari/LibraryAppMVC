using LibraryAppMVC.API.Data;
using LibraryAppMVC.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppMVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BorrowController(LibraryDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> BorrowBook(int bookId, [FromBody] string borrowerName)
    {
        var book = await context.Books.FindAsync(bookId);

        if (book == null || !book.IsAvailable)
        {
            return BadRequest("Book is not available for borrowing.");
        }
        book.IsAvailable = false;

        var record = new BorrowRecord
        {
            BookId = bookId,
            BorrowerName = borrowerName,
            BorrowDate = DateTime.Now
        };

        context.BorrowRecords.Add(record);
        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("return/{bookId}")]
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        var book = await context.Books.FindAsync(bookId);
        if (book == null || book.IsAvailable)
        {
            return BadRequest("Book was not borrowed or does not exist.");
        }

        var record = await context.BorrowRecords
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.ReturnDate == null);

        if (record == null)
        {
            return NotFound("No active borrow record found for this book.");
        }

        // ۲. ثبت تاریخ بازگشت و آزاد کردن وضعیت کتاب
        record.ReturnDate = DateTime.Now;
        book.IsAvailable = true;

        await context.SaveChangesAsync();

        return Ok();
    }
}
