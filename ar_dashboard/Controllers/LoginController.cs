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
        private readonly ICosmosDbService _userDbService;
        private readonly ICosmosDbService _authenDbService;
        private readonly IConfiguration _configuration;
        public LoginController(ICosmosDbService userDbService, ICosmosDbService authenDbService, IConfiguration configuration)
        {
            _userDbService = userDbService ?? throw new ArgumentNullException(nameof(userDbService));
            _authenDbService = authenDbService ?? throw new ArgumentNullException(nameof(authenDbService));
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginForm loginForm)
        {
            string username = loginForm.UserName;
            string password = loginForm.Password;
            if (username == null || password == null) return BadRequest("Not Null " + username + password);

            if (!RegexChecker.checkAuthString(username)) return BadRequest("Valid username");
            if (!RegexChecker.checkAuthString(password)) return BadRequest("Valid password");

            UserModel login = new UserModel();
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

        private async Task<UserModel> AuthenticateUser(UserModel login)
        {
            UserModel user = null;

            // find user in database and compare password input with password db
            /* var accs = null; // try to get db user _context.Auths.Where(user => user.UserName == login.UserName).ToList();
             if (accs == null || accs.Count == 0) return user;
             Auths acc = accs[0];

             if (!HashPassword.VerifyHashedPassword(acc.Password, login.Password)) return user;
            */

            user = (UserModel) await _authenDbService.GetAsync(login.UserId);
           /* var role = (from u in _context.Users
                        join r
in _context.Roles on u.IdRole equals r.IdRole
                        where u.IdUser == acc.IdUser
                        select r.RoleName).ToList();
            user = new UserModel
            {
                IdUser = acc.IdUser,
                UserName = acc.UserName,
                EmailAddress = acc.Email,
                Password = acc.Password,
                Role = role[0].ToString(),
            };*/

            return user;
        }

        private string GenerateJSONWebToken(UserModel userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, userinfo.UserId),
                new Claim(JwtRegisteredClaimNames.Email, userinfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        [Authorize]
        [HttpPost]
        [Route("TestAuth")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            var role = claim[1].Value;
            return "Welcom to: " + userName + " " + role;
        }

        [HttpGet("GetValue")]
        [Route("api/mmmpost")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Value1", "Value2", "Value3" };
        }

        [Authorize]
        [HttpPost]
        [Route("RefreshToken")]
        public ActionResult<IEnumerable<string>> RefreshToken(string oldToken)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userName = claim[0].Value;
                var role = claim[1].Value;
                UserModel user = new UserModel
                {
                    UserId = claim[2].Value,
                    UserName = claim[0].Value,
                    EmailAddress = claim[3].Value,
                    Password = "",
                    Role = Int32.Parse(claim[1].Value),
                };

                var newToken = GenerateJSONWebToken(user);
                return Ok(new { token = newToken }); ;
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e);
            }
        }
    }
}
