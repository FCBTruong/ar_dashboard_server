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
                if (!RegexChecker.checkAuthString(email)) return Ok(
                    new ClientReceiveSignup(SignupController.ERROR_INPUT));
                if (!RegexChecker.checkAuthString(signupForm.Password)) return Ok(
                    new ClientReceiveSignup(SignupController.ERROR_INPUT));


                if (await checkUsernameExsit(signupForm.UserName)) return Ok(
                    SignupController.ERROR_SAME_ACCOUNT);
                string passwordHash = HashPassword.getHashPassCode(signupForm.Password);
                string pass = signupForm.Password;
                signupForm.Password = passwordHash;

                if (await createAccount(signupForm)) return Ok(new LoginController(_databaseController, _config).Login(
                    new LoginForm
                    {
                        Email = email,
                        Password = pass,
                    }));
                return Ok(new ClientReceiveSignup(
                    SignupController.SUCCESS));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        private async Task<bool> checkUsernameExsit(string username)
        {
            List<AuthenModel> authList = (await _authenDbService.GetMultipleAsync($"SELECT * FROM c WHERE c.username = '{username}'")).ToList();
            foreach (AuthenModel auth in authList)
            {
                if (auth.UserName == username) return true;
            }
            return false;
        }

        private async Task<bool> createAccount(SignupForm signupForm)
        {
            AuthenModel user = new AuthenModel
            {
                UserName = signupForm.UserName,
                Password = signupForm.Password,
                Id = Guid.NewGuid().ToString(),
                Email = signupForm.Email,
                Role = (ushort)UserRole.USER
            };

            // save to db
            await _authenDbService.AddAsync(user);


            return true;
        }
    }
}
