using ar_dashboard.Controllers;
using ar_dashboard.Models;
using ar_dashboard.Models.Authentication;
using ar_dashboard.Models.ClientSendForm;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models.ClientReceiveForm;

namespace Web_BTL_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        public const int SUCCESS = 0;
        public const int ERROR_INPUT = 1;
        public const int ERROR_SAME_ACCOUNT = 2;

        private IConfiguration _config;
        private readonly IAuthenDbService _authenDbService;
        private readonly DatabaseController _databaseController;
        public SignupController(IConfiguration config, DatabaseController databaseController)
        {
            this._authenDbService = databaseController.AuthenDbService;
            this._config = config;
            _databaseController = databaseController;
        }

        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] SignupForm signupForm)
        {
            try
            {
                if (signupForm == null) return BadRequest("form is null");
                string email = signupForm.Email;
              
                if (!RegexChecker.checkAuthString(email)) return BadRequest(
                    new ClientReceiveSignup(ERROR_INPUT, signupForm.Email));
                if (!RegexChecker.checkAuthString(signupForm.Password)) return Ok(
                    new ClientReceiveSignup(ERROR_INPUT, signupForm.Password));


                if (await checkUsernameExist(signupForm.Email)) return Conflict(
                    SignupController.ERROR_SAME_ACCOUNT);
                string passwordHash = HashPassword.getHashPassCode(signupForm.Password);
                string pass = signupForm.Password;
                signupForm.Password = passwordHash;

                if (await createAccount(signupForm)) return await new LoginController(_databaseController, _config).Login(
                    new LoginForm
                    {
                        Email = email,
                        Password = pass,
                    });
                return Ok(new ClientReceiveSignup(
                    SignupController.SUCCESS, ""));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        private async Task<bool> checkUsernameExist(string email)
        {
            List<AuthenModel> authList = (await _authenDbService.GetMultipleAsync($"SELECT * FROM c WHERE c.email = '{email}'")).ToList();
            foreach (AuthenModel auth in authList)
            {
                if (auth.Email == email) return true;
            }
            return false;
        }

        private async Task<bool> createAccount(SignupForm signupForm)
        {
            var role = (ushort)UserRole.USER;
            // fix tam
            if (signupForm.Email.Equals("nguyenhuytruong9112k@gmail.com"))
            {
                role = (ushort)UserRole.ADMIN;
            }

            AuthenModel user = new AuthenModel
            {
                UserName = signupForm.UserName,
                Password = signupForm.Password,
                Id = Guid.NewGuid().ToString(),
                Email = signupForm.Email,
                Role = role
            };

            // save to db
            await _authenDbService.AddAsync(user);


            return true;
        }
    }
}
