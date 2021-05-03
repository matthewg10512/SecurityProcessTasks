using Microsoft.EntityFrameworkCore;
using SecurityProcessTasks.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityProcessTasks.DbContexts
{
    public class SecurityContext: DbContext 
    {


        public SecurityContext(DbContextOptions<SecurityContext> options)
           : base(options)
        {

        }


        public DbSet<SecurityTasks> SecurityTasks { get; set; }

    }
}
