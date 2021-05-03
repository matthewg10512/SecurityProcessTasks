using System;
using System.Collections.Generic;
using System.Text;

using static SecurityProcessTasks.Constants;

namespace SecurityProcessTasks.Services.Environment
{
    public class EnvironmentService : IEnvironmentService
    {
        public EnvironmentService()
        {
            EnvironmentName = System.Environment.GetEnvironmentVariable(EnvironmentVariables.AspnetCoreEnvironment)
                ?? Environments.Production;
        }

        public string EnvironmentName { get; set; }
    }
}
