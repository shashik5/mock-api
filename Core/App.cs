
using System;
using UserManagement;
using UserManagement.Models;

namespace Core
{
    public class AppConfig
    {
        public string DBConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IApp
    {
        IUserManager GetUserManager();
    }

    public class App : IApp
    {
        private IUserManager UserManager { get; set; }

        public App(AppConfig config)
        {
            UserManager = new UserManager(new UserManagerConfig()
            {
                ConnectionString = config.DBConnectionString,
                DatabaseName = config.DatabaseName
            });
        }

        public IUserManager GetUserManager()
        {
            return UserManager;
        }
    }
}
