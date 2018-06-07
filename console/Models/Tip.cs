using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace footytips.Models
{
    public class Tip
    {
        public Tip(List<Score> scores, NrlMatches game)
        {
            var homeTeamScore = scores.Sum(x => x.HomeTeam);
            var awayTeamScore = scores.Sum(x => x.AwayTeam);

            Scores = scores;

            if (homeTeamScore >= awayTeamScore)
            {
                Score = homeTeamScore;
                ScoreDiff = homeTeamScore - awayTeamScore;
                Selection = game.HomeTeam.NickName;
            }
            else
            {
                Score = awayTeamScore;
                ScoreDiff = awayTeamScore - homeTeamScore;
                Selection = game.AwayTeam.NickName;
            }
        }

        private List<Score> Scores { get; }
        private string Selection { get; }
        private int Score { get; }
        private int ScoreDiff { get; }

        public override string ToString()
        {
            var strBuilder = new StringBuilder($"{Selection} - S: {Score}, Diff: {ScoreDiff} ");

//            strBuilder.AppendLine();
//            foreach (var score in Scores)
//                strBuilder.AppendLine($"    - {score.RuleName} - H: {score.HomeTeam}, A: {score.AwayTeam}");

            return strBuilder.ToString();
        }
    }
}