namespace ffxivList.Models
{
    public class AllUserLevemete
    {
        public int UserLevemeteId { get; set; }
        public int LevemeteId { get; set; }
        public string UserId { get; set; }
        public bool IsComplete { get; set; }
        public string LevemeteName { get; set; }
        public int LevemeteLevel { get; set; }
    }
}