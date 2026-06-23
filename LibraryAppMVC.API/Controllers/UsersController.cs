using LibraryAppMVC.API.Data;
using LibraryAppMVC.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppMVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(LibraryDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        List<User> users = await context.Users.ToListAsync();
        if (users.Count == 0)
            return BadRequest("Any user is not fournd");
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Created();
    }
}
