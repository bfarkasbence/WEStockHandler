using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEStockHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace WEStockHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginController(SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel userData)
        {
            var user = await _userManager.FindByNameAsync(userData.UserName);

            if (user != null)
            {
                var SignInResult = await _signInManager.PasswordSignInAsync(user, userData.Password, false, false);


                if (SignInResult.Succeeded) { return Ok("Logged in"); }

                return NotFound("Wrong password!");


            }
            return NotFound("User not found");


        }

    }
}
