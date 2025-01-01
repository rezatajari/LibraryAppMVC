using LibraryAppMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LibraryAppMVC.Data
{
    public class LibraryDB : IdentityDbContext<User>
    {
        public LibraryDB(DbContextOptions<LibraryDB> options)
        : base(options) { }

        public DbSet<Book> Books { get; set; }

        // public DbSet<User> Users { get; set; } is removed because you are now using ApplicationUser
        // public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }

}
