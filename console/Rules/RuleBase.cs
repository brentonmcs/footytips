using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public abstract class RuleBase : IRule
    {
        protected abstract string RuleName { get; }

        public Score GetScoreClass(NrlMatches game, IList<ClubLadder> dataLadder, IList<NrlMatches> lastRound)
        {
            var scores = GetScore(game, dataLadder, lastRound);
            return new Score {RuleName = RuleName, HomeTeam = scores.Item1, AwayTeam = scores.Item2};
        }

        protected abstract (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound);

        protected static bool DidWin(string clubName, IList<NrlMatches> matcheses)
        {
            return DidWin(clubName, matcheses.GetLastRound(clubName));
        }

        private static bool DidWin(string clubName, NrlMatches match)
        {
            if (match == null)
            {
                return true;
            }
            if (match.HomeTeam.NickName == clubName) return match.HomeTeam.Score > match.AwayTeam.Score;

            return match.AwayTeam.Score > match.HomeTeam.Score;
        }
    }
}