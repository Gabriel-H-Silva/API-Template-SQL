using System.Security.Claims;

namespace ManagerIO.Services
{
    public interface ITokenServices
    {
        Task<string> GenerateAccessToken(IEnumerable<Claim> claims);
        Task<string> GenerateRefreshToken();
        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
