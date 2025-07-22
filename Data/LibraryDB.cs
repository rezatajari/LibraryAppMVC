using LibraryAppMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LibraryAppMVC.Data
{
    public class LibraryDb: IdentityDbContext<User>
    {
        public LibraryDb(DbContextOptions<LibraryDb> options) : base(options)
        {
        }
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
                .HasMaxLength(100);

            modelBuilder.Entity<Book>()
                .Property(b => b.Author)
                .HasMaxLength(100);
        }
    }

}
