namespace ffxivList.Models
{
    public class AllUserCraft
    {
        public int UserCraftId { get; set; }
        public int CraftId { get; set; }
        public string UserId { get; set; }
        public bool IsComplete { get; set; }
        public string CraftName { get; set; }
        public int CraftLevel { get; set; }
    }
}
