using System.Collections.Generic;

namespace footytips.Models
{
    public class CombinedData
    {
        public IEnumerable<ClubLadder> Ladder { get; set; }
        public IEnumerable<NrlMatches> LastRound { get; set; }
        public IEnumerable<NrlMatches> CurrentRound { get; set; }
    }

    public class Score
    {
        public int HomeTeam { get; set; }
        public int AwayTeam { get; set; }
        public string RuleName { get; set; }
    }
}