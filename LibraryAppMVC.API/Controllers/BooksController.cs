using Microsoft.AspNetCore.Mvc;
using LibraryAppMVC.API.Data;

namespace LibraryAppMVC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(LibraryDbContext context) : ControllerBase
    {
    }
}