using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<BookService> _logger;
        public AccountService(IAccountRepository accountRepository, ILogger<BookService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<User> Login(LoginViewModel model)
        {
            _logger.LogInformation("Login servies is run");
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

        public async Task<User> GetUserById(int? id)
        {
            return await _accountRepository.GetUserById(id);
        }

        public async Task EditProfileUser(int? userId, ProfileViewModel model)
        {
            var user = await _accountRepository.GetUserById(userId);
            user.Email = model.Email;
            user.UserName = model.UserName;

            if (model.ProfilePicture != null)
            {
                // Define the folder to save the file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);

                // Save the file to the folder
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePicturePath = "/uploads/" + fileName;
            }

            await _accountRepository.UpdateUser(user);
        }

        public async Task<bool> EmailEditExist(int? userId, string email)
        {
            var emailExists = await _accountRepository.EmailExists(userId, email);

            if (emailExists)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveUser(int? userId)
        {

            var user = await _accountRepository.GetUserById(userId);
            return await _accountRepository.RemoveUser(user);
        }
    }
}
