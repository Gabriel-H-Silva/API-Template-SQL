using ManagerApi.Model.DM;

namespace ManagerIO.Business.Interfaces
{
    public interface ILoginBusiness
    {
        Task<TokenDM> ValidateCredentials(UsersDM user);
        Task<TokenDM> ValidateCredentials(TokenDM token);
        Task<bool>  RevokeToken(string userName);
    }
}
