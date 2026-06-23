using LibraryAppMVC.API.Data;
using LibraryAppMVC.Shared.Models;
using Microsoft.AspNetCore.Identity;
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

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        var userExists = await context.Users.AnyAsync(u => u.Email == user.Email);
        if (userExists)
        {
            return BadRequest("Email is already registered.");
        }

        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, user.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User loginInfo)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginInfo.Email);
        if (user == null)
        {
            return BadRequest("Invalid Email or Password.");
        }

        var passwordHasher = new PasswordHasher<User>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, loginInfo.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return BadRequest("Invalid Email or Password.");
        }

        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }
}
