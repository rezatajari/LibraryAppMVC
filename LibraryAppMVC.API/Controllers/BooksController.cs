using LibraryAppMVC.API.Data;
using LibraryAppMVC.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppMVC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(LibraryDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await context.Books.ToListAsync();
            return Ok(books);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // برگرداندن وضعیت 201 Created به همراه آدرس دسترسی به این کتاب و خود شیء ساخته شده
            return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
        }
    }
}