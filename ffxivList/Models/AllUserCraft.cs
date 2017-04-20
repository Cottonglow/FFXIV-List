namespace ffxivList.Models
{
    public class AllUserCraft
    {
        public int UserCraftID { get; set; }
        public int CraftID { get; set; }
        public string UserID { get; set; }
        public bool IsComplete { get; set; }
        public string CraftName { get; set; }
        public int CraftLevel { get; set; }
    }
}
