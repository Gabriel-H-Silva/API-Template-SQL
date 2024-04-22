using ManagerIO.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagerApi.Model.DM;

namespace ManagerIO.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBusiness;

        public AuthController(ILoginBusiness loginBusiness)
        {
            _loginBusiness = loginBusiness;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Signin([FromBody] UsersDM user)
        {
            if(user == null) return BadRequest("Invalid client request");
            var token = _loginBusiness.ValidateCredentials(user);
            if (token == null) return Unauthorized();
            return Ok(token);
        }
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] TokenDM TokenDM)
        {
            if (TokenDM is null) return BadRequest("Invalid client request");
            var token = _loginBusiness.ValidateCredentials(TokenDM);
            if (token == null) return BadRequest("Invalid client request");
            return Ok(token);
        }
        [HttpGet]
        [Route("revoke")]
        [Authorize("Bearer")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var result = _loginBusiness.RevokeToken(username);

            if (!result) return BadRequest("Invalid client request");
            return NoContent();
        }
    }
}
