using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class HomeAdvantageRule : RuleBase
    {
        protected override string RuleName => "Home Advantage Rule";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var ladder = dataLadder.GetLadder(game.HomeTeam.NickName);

            if (ladder.HomeForm > 0) return (20, 0);

            return ladder.HomeForm == 0 ? (10, 0) : (0, 0);
        }
    }
}