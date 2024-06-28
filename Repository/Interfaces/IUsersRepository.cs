using ManagerApi.Model;

namespace ManagerApi.Repository.Interfaces
{
    public interface IUsersRepository
    {
        Task<bool> CheckExistUserById(int id);
        Task<Users?> FindUserById(int id);
        Task<Users?> FindUserByName(string name);
        Task<Users?> ValidadeCredentials(string userName);
        Task<List<Users?>> FindAll();
        Task<bool> RevokeToken(string userName);
        Task<Users?> RefreshUserInfo(Users user);
        Task<Users?> ValidadeCredentials(Users user);
    }
}
