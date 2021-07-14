using Bilbayt.Models;
using Bilbayt.RequestModels;
using Bilbayt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bilbayt.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;
        
        public LoginController(ILoginService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                var user = await _loginService.GetByUserName(login.UserName);
                if (user == null || user.Id == null)
                    return NotFound("User not found.");

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(login.Password);

                bool verified = BCrypt.Net.BCrypt.Verify(login.Password, passwordHash);
                if (!verified)
                    return BadRequest("User Name and Password Does not match");
                else
                {
                    var token = _loginService.GenerateJSONWebToken();
                    TempData["fullname"] = $"{user.FirstName} {user.LastName}";

                    return Ok(new LoginResultModel { 
                     FirstName = user.FirstName,
                     LastName = user.LastName,
                     Token = token
                    });                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            try
            {
                Login login = new Login();
                login.FirstName = model.FirstName;
                login.LastName = model.LastName;
                login.UserName = model.UserName;
                login.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var result = await _loginService.Create(login);
                if (result!= null)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
