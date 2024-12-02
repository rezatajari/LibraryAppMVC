﻿using LibraryAppMVC.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryAppMVC.Data
{
    public class LibraryDB : DbContext
    {
        public LibraryDB(DbContextOptions<LibraryDB> options)
        : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}