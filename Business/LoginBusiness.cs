using ManagerIO.Business.Interfaces;
using ManagerIO.Configurations;
using ManagerIO.Services;
using ManagerIO.Repository.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ManagerApi.Model;
using ManagerApi.Model.DM;

namespace ManagerIO.Business
{
    public class LoginBusiness : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfiguration _configuration;

        private UsersRepository _repository;

        public LoginBusiness(TokenConfiguration configuration, UsersRepository repository, ITokenServices tokenServices)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenServices = tokenServices;
        }

        private readonly ITokenServices _tokenServices;
        public TokenDM ValidateCredentials(UsersDM userCredentials)
        {
            Users loginData = new Users();
            loginData.name = userCredentials.name;
            loginData.password = userCredentials.password;

            var user = _repository.ValidadeCredentials(loginData);
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.name)
            };

            var accessToken = _tokenServices.GenerateAccessToken(claims);
            var refreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenDM(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public TokenDM ValidateCredentials(TokenDM token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenServices.GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity.Name;

            var user = _repository.ValidadeCredentials(username);

            if (
                user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now
            ) return null;

            accessToken = _tokenServices.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenDM(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public bool RevokeToken(string userName)
        {
            return _repository.RevokeToken(userName);
        }
    }
}
