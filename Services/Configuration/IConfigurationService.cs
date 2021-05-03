using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityProcessTasks.Services.Configuration
{
    public interface IConfigurationService
    {
        IConfiguration GetConfiguration();
    }
}
