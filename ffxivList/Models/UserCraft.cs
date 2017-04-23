namespace ffxivList.Models
{
    public class UserCraft
    {
        public int UserCraftId { get; set; }
        public int CraftId { get; set; }
        public string UserId { get; set; }
        public bool IsComplete { get; set; }
    }
}
