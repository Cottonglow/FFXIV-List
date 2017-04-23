namespace ffxivList.Models
{
    public class UserQuest
    {
        public int UserQuestId { get; set; }
        public int QuestId { get; set; }
        public string UserId { get; set; }
        public bool IsComplete { get; set; }
    }
}
