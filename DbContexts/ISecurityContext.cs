using Microsoft.EntityFrameworkCore;
using SecurityProcessTasks.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityProcessTasks.DbContexts
{
    interface ISecurityContext
    {
         DbSet<SecurityTasks> SecurityTasks { get; set; }



    }
}
