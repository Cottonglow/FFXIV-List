namespace ffxivList.Models
{
    public class AllUserLevemete
    {
        public int UserLevemeteID { get; set; }
        public int LevemeteID { get; set; }
        public string UserID { get; set; }
        public bool IsComplete { get; set; }
        public string LevemeteName { get; set; }
        public int LevemeteLevel { get; set; }
    }
}