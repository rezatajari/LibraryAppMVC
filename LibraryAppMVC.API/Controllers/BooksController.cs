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
    }
}