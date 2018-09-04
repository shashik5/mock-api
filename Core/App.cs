
using System;

namespace Core
{
    public interface IAppConfig
    {
        string DBConnectionString { get; set; }
    }

    interface IApp
    {
    }

    public class App : IApp
    {
        private string DBConnectionString { get; set; }

        public App(IAppConfig config)
        {
            DBConnectionString = config.DBConnectionString;
        }
    }
}
