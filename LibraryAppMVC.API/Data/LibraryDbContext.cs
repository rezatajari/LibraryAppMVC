using Microsoft.EntityFrameworkCore;
using LibraryAppMVC.Shared.Models;

namespace LibraryAppMVC.API.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
}