using ManagerApi.Model;
using ManagerIO.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerIO.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Authorize("Bearer")]
    [Route("api/[controller]/v{version:apiVersion}")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private IUsersBusiness _userBusiness;
        public UsersController(ILogger<UsersController> logger, IUsersBusiness userBusiness)
        {
            _logger = logger;
            _userBusiness = userBusiness;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userBusiness.FindAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var person = _userBusiness.FindById(id);

            if (person == null) return NotFound();

            return Ok(person);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Users person)
        {
            if (person == null) return BadRequest();

            return Ok(_userBusiness.Create(person));
        }


        [HttpPut]
        public IActionResult Put([FromBody] Users person)
        {

            if (person == null) return BadRequest();

            return Ok(_userBusiness.Update(person));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _userBusiness.Delete(id);

            return NoContent();
        }
    }
}
