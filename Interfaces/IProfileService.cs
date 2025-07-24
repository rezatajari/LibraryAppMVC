using LibraryAppMVC.Utilities;
using LibraryAppMVC.ViewModels;

namespace LibraryAppMVC.Interfaces
{
    public interface IProfileService
    {
        Task<ResultTask<ProfileViewModel>> GetUserByEmail(string email);
        Task<ResultTask<EditProfileViewModel>> GetEditProfile(string email);
        Task<ResultTask<bool>> EditProfileUser(EditProfileViewModel model, string userId);
    }
}
