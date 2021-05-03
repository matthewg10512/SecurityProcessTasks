using SecurityProcessTasks.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityProcessTasks.Services.Repository
{
   public interface ISecurityRepository
    {
        SecurityTasks GetTasks(string taskName);
        bool Save();

        void UpdateTasks(SecurityTasks task);

    }
}
