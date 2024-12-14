using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<User> Login(LoginViewModel model)
        {
            return await _accountRepository.Login(model);
        }
        public async Task<bool> CheckUserExist(string email)
        {
            return await _accountRepository.CheckUserExist(email);
        }

        public async Task<bool> Register(RegisterViewModel model)
        {
            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                BirthdayDate = model.BirthdayDate
            };

            await _accountRepository.Register(user);
            return true;
        }
    }
}
