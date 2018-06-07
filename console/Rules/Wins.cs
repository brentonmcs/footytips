using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class Wins : RuleBase
    {
        protected override string RuleName => "Wins for the Season";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var homeLadder = dataLadder.GetLadder(game.HomeTeam.NickName);
            var awayLadder = dataLadder.GetLadder(game.AwayTeam.NickName);

            if (homeLadder.Wins == awayLadder.Wins)
            {
                return (0, 0);
            }

            return homeLadder.Wins > awayLadder.Wins ? (10, 0) : (0, 10);
        }
    }
}