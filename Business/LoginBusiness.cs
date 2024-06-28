using ManagerIO.Business.Interfaces;
using ManagerIO.Configurations;
using ManagerIO.Services;
using ManagerIO.Repository.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ManagerApi.Model;
using ManagerApi.Model.DM;
using ManagerApi.Repository.Interfaces;

namespace ManagerIO.Business
{
    public class LoginBusiness : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfiguration _configuration;

        private IUsersRepository _repository;

        public LoginBusiness(TokenConfiguration configuration, IUsersRepository repository, ITokenServices tokenServices)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenServices = tokenServices;
        }

        private readonly ITokenServices _tokenServices;
        public async Task<TokenDM> ValidateCredentials(UsersDM userCredentials)
        {
            Users loginData = new Users();
            loginData.name = userCredentials.name;
            loginData.password = userCredentials.password;

            var user = await _repository.ValidadeCredentials(loginData);
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.name)
            };

            var accessToken = await _tokenServices.GenerateAccessToken(claims);
            var refreshToken = await _tokenServices.GenerateRefreshToken();

            user.refreshToken = refreshToken;
            user.refreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            await _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenDM
            (
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
            );
        }

        public async Task<TokenDM> ValidateCredentials(TokenDM token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = await _tokenServices.GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity.Name;

            var user = await _repository.ValidadeCredentials(username);

            if (
                user == null ||
                user.refreshToken != refreshToken ||
                user.refreshTokenExpiryTime <= DateTime.Now
            ) return null;

            accessToken = await _tokenServices.GenerateAccessToken(principal.Claims);
            refreshToken = await _tokenServices.GenerateRefreshToken();

            user.refreshToken = refreshToken;

            await _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenDM
            (
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
            );
        }

        public async Task<bool> RevokeToken(string userName)
        {
            return await _repository.RevokeToken(userName);
        }
    }
}
