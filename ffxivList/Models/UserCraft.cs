using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffxivList.Models
{
    public class UserCraft
    {
        public int UserCraftID { get; set; }
        public int CraftID { get; set; }
        public string UserID { get; set; }
        public bool IsComplete { get; set; }
    }
}
