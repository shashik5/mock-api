
using System;
using Authentication;
using Authentication.Models;
using Core.Models;

namespace Core
{
    public class AppConfig
    {
        public string DBConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UserTableName { get; set; }
        public string CoreTableName { get; set; }
    }

    interface IApp
    {
    }

    public class App : IApp
    {
        private AuthenticationManager AuthManager { get; set; }

        public App(AppConfig config)
        {
            AuthManager = new AuthenticationManager(new AuthenticationManagerConfig()
            {
                ConnectionString = config.DBConnectionString,
                DatabaseName = config.DatabaseName,
                TableName = config.UserTableName
            });
        }

        public bool ValidateUser(string userName, string authCode)
        {
            return AuthManager.Validate(userName, authCode);
        }

        public UserDetails GetUserDetails(string authCode)
        {
            User userDetails = AuthManager.GetUserDetails(authCode);
            return new UserDetails
            {
                FullName = userDetails.FullName,
                UserName = userDetails.UserName,
                AccountType = userDetails.AccountType,
                DOB = userDetails.DOB
            };
        }
    }
}
