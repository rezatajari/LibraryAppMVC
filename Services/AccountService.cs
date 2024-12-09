using LibraryAppMVC.Interfaces;
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

        public bool Login(LoginViewModel model)
        {
            return _accountRepository.Login(model);
        }
    }
}
