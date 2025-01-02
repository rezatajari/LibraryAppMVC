using LibraryAppMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LibraryAppMVC.Data
{
    public class LibraryDb(DbContextOptions<LibraryDb> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Book> Books { get; set; }

        // public DbSet<User> Users { get; set; } is removed because you are now using ApplicationUser
        // public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------- User ---------
            modelBuilder.Entity<User>()
                .Property(u => u.ProfilePicturePath)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // -------- Book ---------
            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .HasMaxLength(50);

            modelBuilder.Entity<Book>()
                .Property(b => b.Author)
                .HasMaxLength(50);

            modelBuilder.Entity<Book>()
                .Property(b => b.UserId)
                .HasMaxLength(100);
        }
    }

}
