using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class BadTravelingTeam : RuleBase
    {
        protected override string RuleName => "Bad Traveling Team";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var ladder = dataLadder.GetLadder(game.AwayTeam.NickName);

            if (ladder.AwayForm < -4) //Need to change this to < 75% of games
                return (20, 0);

            if (ladder.AwayForm <= -2) //Need to change this to < 40% of games
                return (10, 0);

            return ladder.AwayForm <= 0 ? (0, 8) : (0, 15);
        }
    }
}