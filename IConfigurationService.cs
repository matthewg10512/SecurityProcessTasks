using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityProcessTasks
{
    public interface IConfigurationService
    {
        IConfiguration GetConfiguration();
    }
}
