using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class AwayPointsRule : RuleBase
    {
        protected override string RuleName => "Away Points Rule";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var homeTeam = dataLadder.GetLadder(game.HomeTeam.NickName);
            var awayTeam = dataLadder.GetLadder(game.AwayTeam.NickName);

            return homeTeam.AwayForm < awayTeam.AwayForm ? (10, 0) : (0, 10);
        }
    }
}