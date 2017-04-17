namespace ffxivList.Models
{
    public class AllUserQuest
    {
        public int UserQuestID { get; set; }
        public int QuestID { get; set; }
        public string UserID { get; set; }
        public bool IsComplete { get; set; }
        public string QuestName { get; set; }
        public int QuestLevel { get; set; }
    }
}