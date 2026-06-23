using Microsoft.EntityFrameworkCore;
using LibraryAppMVC.Shared.Models;

namespace LibraryAppMVC.API.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BorrowRecord> BorrowRecords { get; set; }
    public DbSet<User> Users { get; set; }
}