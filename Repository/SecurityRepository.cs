using SecurityProcessTasks.DbContexts;
using SecurityProcessTasks.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace SecurityProcessTasks.Repository
{
    public class SecurityRepository : ISecurityRepository   
    {

        public SecurityContext _context { get; }

        public SecurityRepository(SecurityContext context)
        {
            _context = context;
        }

        
        public SecurityTasks GetTasks(string taskName)
        {
            return _context.SecurityTasks.FirstOrDefault(x => x.TaskName == taskName);
        }

        public bool Save()
        {

            return (_context.SaveChanges() >= 0);
        }

        public void UpdateTasks(SecurityTasks task)
        {
            _context.SecurityTasks.Update(task);
            Save();
        }

    }
}
