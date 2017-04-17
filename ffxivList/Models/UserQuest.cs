using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffxivList.Models
{
    public class UserQuest
    {
        public int UserQuestID { get; set; }
        public int QuestID { get; set; }
        public string UserID { get; set; }
        public bool IsComplete { get; set; }
    }
}
