namespace ffxivList.Models
{
    public class UserProfile
    {
        public string ProfileEmail { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileRole { get; set; }
        public int LevemetesCompleted { get; set; }
        public int LevemetesTotal { get; set; }
        public int QuestsCompleted { get; set; }
        public int QuestsTotal { get; set; }
        public int CraftsCompleted { get; set; }
        public int CraftsTotal { get; set; }
    }
}
