using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class LastGameWinRule : RuleBase
    {
        protected override string RuleName => "Last Game Win Rule";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var home = 0;
            var away = 0;

            if (game == null)
            {
                return (0, 0);
            }
            if (DidWin(game.HomeTeam.NickName, lastRound)) home += 10;

            if (DidWin(game.AwayTeam.NickName, lastRound)) away += 10;

            return (home, away);
        }
    }
}