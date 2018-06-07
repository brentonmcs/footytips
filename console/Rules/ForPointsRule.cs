using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class ForPointsRule : RuleBase
    {
        protected override string RuleName => "For Points Rule";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var homeTeam = dataLadder.GetLadder(game.HomeTeam.NickName);
            var awayTeam = dataLadder.GetLadder(game.AwayTeam.NickName);

            return homeTeam.ForPoints > awayTeam.ForPoints ? (10, 0) : (0, 10);
        }
    }
}