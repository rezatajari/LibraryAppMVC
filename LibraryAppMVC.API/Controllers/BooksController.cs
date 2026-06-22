using Microsoft.AspNetCore.Mvc;
using LibraryAppMVC.Shared.Models;

namespace LibraryAppMVC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly List<Book> _books = new()
        {
            new Book { Id = 1, Title = "شاهنامه", Author = "فردوسی", YearPublished = 1010 },
            new Book { Id = 2, Title = "بوف کور", Author = "صادق هدایت", YearPublished = 1937 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            return Ok(_books);
        }
    }
}