using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEStockHandler.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WEStockHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

       
        [HttpGet]
        public async Task<IQueryable<IdentityUser>> GetUsers()
        {
            var users = await Task.Run(() => { return _userManager.Users; });
            return users;
        }

        
        [HttpGet("{userName}")]
        public async Task<ActionResult<IdentityUser>> GetUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) { return NotFound("User not found!"); }

            return Ok(user);
        }

        
        [HttpPost]
        public async Task<ActionResult<IdentityUser>> CreateUser(RegisterModel user)
        {
            var userToCreate = new IdentityUser { UserName = user.UserName, Email = user.Email };
            var restult = await _userManager.CreateAsync(userToCreate, user.Password);
            if (restult.Succeeded) { return CreatedAtAction("createUser", new IdentityUser(), userToCreate); }
            return NotFound("Username or password already in use");

        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) { return NotFound("Wrong user id"); }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) { return Ok("User deleted succesfully!"); }
            return NotFound(result.Errors);
        }
    }
}
