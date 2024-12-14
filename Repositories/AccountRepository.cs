﻿using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppMVC.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly LibraryDB _libraryDB;
        public AccountRepository(LibraryDB libraryDB)
        {
            _libraryDB = libraryDB;
        }


        public async Task<User> Login(LoginViewModel model)
        {
            var user = await _libraryDB.Users
                .Where(u => u.Email == model.Email && u.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                return user;
            }

            return null;
        }

        public async Task<bool> CheckUserExist(string email)
        {
            var user = await _libraryDB.Users.
                 Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            return true;

        }

        public async Task<bool> Register(User newUser)
        {
            await _libraryDB.Users.AddAsync(newUser);
            await _libraryDB.SaveChangesAsync();
            return true;
        }
    }
}