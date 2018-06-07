namespace footytips.Models
{
    public class NrlMatches
    {
        public string MatchMode { get; set; }

        public NrlTeam HomeTeam { get; set; }
        public NrlTeam AwayTeam { get; set; }
    }
}