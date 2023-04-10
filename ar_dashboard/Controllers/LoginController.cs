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
using ar_dashboard.Models;
using ar_dashboard.Models.Authentication;
using ar_dashboard.Services;
using ar_dashboard.Models.ClientSendForm;
using System.Threading.Tasks;

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        private readonly IAuthenDbService _authenDbService;
        private readonly IConfiguration _configuration;
        public LoginController(DatabaseController databaseController, IConfiguration configuration)
        {
            _userDbService = databaseController.UserDbService;
            _authenDbService = databaseController.AuthenDbService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginForm loginForm)
        {
            try
            {
                string username = loginForm.UserName;
                string password = loginForm.Password;
                if (username == null || password == null) return BadRequest("Not Null " + username + password);

                if (!RegexChecker.checkAuthString(username)) return BadRequest("Invalid username");
                if (!RegexChecker.checkAuthString(password)) return BadRequest("Invalid password");

                AuthenModel login = new AuthenModel();
                login.UserName = username;
                login.Password = password;
                IActionResult response = Unauthorized();


                var user = await AuthenticateUser(login);

                if (user != null)
                {
                    var tokenStr = GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        private async Task<AuthenModel> AuthenticateUser(AuthenModel login)
        {
            AuthenModel user = null;

            // find user in database and compare password input with password db
            var accs = (await _authenDbService.GetMultipleAsync($"SELECT * FROM c WHERE  c.username = '{login.UserName}'")).ToArray();
            if (accs.Length == 0) return user;
            var acc = accs[0];

            if (!HashPassword.VerifyHashedPassword(acc.Password, login.Password)) return user;
       
            user = new AuthenModel
            {
                Id = acc.Id,
                UserName = acc.UserName,
                Email = acc.Email,
                Password = acc.Password,
                Role = acc.Role,
            };

            return user;
        }

        private string GenerateJSONWebToken(AuthenModel userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, userinfo.Id),
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userinfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(10080), // 7 days
                signingCredentials: credentials
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        [Authorize]
        [HttpPost]
        [Route("test")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            var role = claim[1].Value;
            return "Welcom to: " + userName + " " + role;
        }


        [Authorize]
        [HttpPost]
        [Route("refresh-token")]
        public ActionResult<IEnumerable<string>> RefreshToken(string oldToken)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userName = claim[0].Value;
                var role = claim[1].Value;
                AuthenModel user = new AuthenModel
                {
                    Id = claim[2].Value,
                    UserName = claim[0].Value,
                    Email = claim[3].Value,
                    Password = "",
                    Role = ushort.Parse(claim[1].Value),
                };

                var newToken = GenerateJSONWebToken(user);
                return Ok(new { token = newToken }); ;
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("verify")]
        public ActionResult<IEnumerable<string>> VerifyToken()
        {
            try
            {              
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e);
            }
        }
    }
}
