﻿using LibraryAppMVC.Models;

namespace LibraryAppMVC.Interfaces
{
    public interface IBookService
    {
        void Add(Book newBook);
        void Remove(Book newBook);
        List<Book> GetAll();
        List<Book> SearchByTitle(string title);
    }
}
