using System;
namespace ar_dashboard.Models.Authentication
{
    public class UserModel
    {

        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public int Role { get; set; }
    }
}
