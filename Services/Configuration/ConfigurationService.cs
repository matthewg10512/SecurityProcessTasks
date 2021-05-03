using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SecurityProcessTasks.Services.Environment;
namespace SecurityProcessTasks.Services.Configuration
{
    class ConfigurationService : IConfigurationService
    {
        public IEnvironmentService EnvService { get; }
        public string CurrentDirectory { get; set; }
        public ConfigurationService(IEnvironmentService envService)
        {
            EnvService = envService;
        }

        public IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{EnvService.EnvironmentName}.json", optional: false, reloadOnChange:true)
                //.AddJsonFile($"appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
