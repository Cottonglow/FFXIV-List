using System.Collections.Generic;

namespace ffxivList.Models
{
    public class ModelContainer
    {
        public List<Craft> Craft { get; set; }
        public IEnumerable<Levemete> Levemete { get; set; }
        public IEnumerable<Quest> Quest { get; set; }
        public IEnumerable<User> User { get; set; }
        public IEnumerable<UserLevemete> UserLevemete { get; set; }
        public IEnumerable<UserQuest> UserQuest { get; set; }
        public List<UserCraft> UserCraft { get; set; }
        public IEnumerable<UserProfile> UserProfile { get; set; }
    }
}
