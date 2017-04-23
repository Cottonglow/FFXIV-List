namespace ffxivList.Models
{
    public class AllUserQuest
    {
        public int UserQuestId { get; set; }
        public int QuestId { get; set; }
        public string UserId { get; set; }
        public bool IsComplete { get; set; }
        public string QuestName { get; set; }
        public int QuestLevel { get; set; }
    }
}