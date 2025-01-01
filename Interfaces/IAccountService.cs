﻿using System.Diagnostics.Eventing.Reader;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LibraryAppMVC.Interfaces
{
    public interface IAccountService
    {
        //------------------ Account Services ------------------//
        Task<ResultTask<string>> LogIn(LoginViewModel model);
        Task<ResultTask<bool>> Registration(RegisterViewModel model);
        Task<ResultTask<bool>> ConfirmationEmailProcess(string userId, string token);
        Task<ResultTask<bool>> DeleteAccount(string email);

        //------------------ Profile Services ------------------//
        Task<ResultTask<ProfileViewModel>> GetUserByEmail(string email);
        Task<ResultTask<bool>> EditProfileUser(ProfileViewModel model,string userId);
    }
}
