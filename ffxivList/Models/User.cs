namespace ffxivList.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public int UserLevemetesCompleted { get; set; }
        public int UserCraftsCompleted { get; set; }
        public int UserQuestsCompleted { get; set; }
    }
}
