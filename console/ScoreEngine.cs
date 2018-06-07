using System.Collections.Generic;
using System.Linq;
using footytips.Models;
using footytips.Rules;

namespace footytips
{
    public class ScoreEngine
    {
        private readonly List<IRule> _ruleList;

        public ScoreEngine ()
        {
            _ruleList = new List<IRule>
            {
                new HomeAdvantageRule (),
                new BadTravelingTeam (),
                new AwayPointsRule (),
                new ForPointsRule (),
                new LastGameWinRule (),
                new LastGameBigLoss (),
                new LastGameLostTop4 (),
                new Wins ()
            };
        }

        public Tip GetTip (NrlMatches game, IList<ClubLadder> dataLadder, IList<NrlMatches> dataLastRound)
        {
            var scores = new List<Score> ();
            _ruleList.ForEach (x => scores.Add (x.GetScoreClass (game, dataLadder, dataLastRound)));

            return new Tip (scores, game);
        }
    }

    public static class Helpers
    {
        public static ClubLadder GetLadder (this IList<ClubLadder> ladders, string clubName)
        {
            return ladders.FirstOrDefault (x => x.Name == clubName);
        }

        public static NrlMatches GetLastRound (this IList<NrlMatches> matcheses, string clubName)
        {
            return matcheses.FirstOrDefault (x => x.AwayTeam.NickName == clubName || x.HomeTeam.NickName == clubName);
        }
    }
}