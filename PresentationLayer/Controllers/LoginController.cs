using DomainLayer.Data;
using DomainLayer.Datas;
using DomainLayer.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;
using MiddlewareNamespace = Layers.Middleware;

namespace Layers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MiddlewareNamespace.Authentication _middleware;
        private readonly IUnitOfWork _db;

        public LoginController(IConfiguration configuration, MiddlewareNamespace.Authentication middleware, IUnitOfWork db)
        {
            _configuration = configuration;
            _middleware = middleware;
            _db = db;
        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid user credentials");
            }

            // Validate user credentials
            var authenticatedUser = _db.User.FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
            if (authenticatedUser == null)
            {
                return BadRequest("Invalid username or password");
            }
            else
            {
                var token = _middleware.GenerateJwtToken(user);
                return Ok(token);
            }
            return BadRequest("try again");

            //var token = _middleware.generatejwttoken(user);
            //return ok(token);
        }
    }
}
