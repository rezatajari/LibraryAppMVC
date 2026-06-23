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

        // بررسی اینکه آیا کتاب وجود دارد و در حال حاضر موجود است یا خیر
        if (book == null || !book.IsAvailable)
        {
            return BadRequest("Book is not available for borrowing.");
        }

        // ۱. تغییر وضعیت کتاب به امانت داده شده
        book.IsAvailable = false;

        // ۲. ثبت رکورد جدید امانت
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
}
