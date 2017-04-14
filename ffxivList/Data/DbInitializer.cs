using ffxivList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffxivList.Data
{
    public class DbInitializer
    {
        public static void Initialize(FFListContext context)
        {
            context.Database.EnsureCreated();

            //// Look for any students.
            //if (context.Levemetes.Any())
            //{
            //    return;   // DB has been seeded
            //}

            //var levemete = new Levemete { ID = 5, IsComplete = false, Level = 1, Name = "test" };
            //context.Levemetes.Add(levemete);
            //context.SaveChanges();
        }
    }
}
