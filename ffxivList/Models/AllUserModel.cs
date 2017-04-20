using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffxivList.Models
{
    public class AllUserModel
    {
        public List<AllUserQuest> AllUserQuests { get; set; }
        public List<AllUserCraft> AllUserCrafts { get; set; }
        public List<AllUserLevemete> AllUserLevemetes { get; set; }

        public List<Levemete> Levemetes { get; set; }
        public List<Quest> Quests { get; set; }
        public List<Craft> Crafts { get; set; }

        public User User { get; set; }
    }
}
