using DomainLayer.Data;
using DomainLayer.DomainModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using PresentationLayer.ViewModels;
using MiddlewareNamespace = Layers.Middleware;

namespace Layers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MiddlewareNamespace.Authentication _middleware;
        private readonly UserDbContext _userDbContext;

        public RegisterController(IConfiguration configuration, MiddlewareNamespace.Authentication middleware, UserDbContext db)
        {
            _configuration = configuration;
            _middleware = middleware;
            _userDbContext = db;
        }
        [HttpPost]
        public IActionResult Register([FromBody]RegisterViewModel user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(user);
            }
            if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password)) 
            {
                return BadRequest("Invalid user");
            }
            if(_userDbContext.Users.Any(u=>u.UserName==user.UserName)|| _userDbContext.Users.Any(e => e.Email == user.Email)) 
            {
                return BadRequest("user already Exist and please use diffrent email");
            }
            var newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CreatedDate = DateTime.UtcNow,

            };
            _userDbContext.Add(newUser);
            _userDbContext.SaveChanges();

            return Ok(newUser);
        }
      
    }
}
