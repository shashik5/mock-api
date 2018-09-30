
using System;
using Authentication;

namespace Core
{
    public class AppConfig
    {
        public string DBConnectionString { get; set; }
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
                DatabaseName = config.UserTableName
            });
        }
    }
}
