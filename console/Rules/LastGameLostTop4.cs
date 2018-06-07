using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class LastGameLostTop4 : RuleBase
    {
        protected override string RuleName => "Last Game Lost to Top4";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            var home = GetTeamScore(dataLadder, lastRound, game.HomeTeam.NickName);
            var away = GetTeamScore(dataLadder, lastRound, game.AwayTeam.NickName);

            return (home, away);
        }

        private static int GetTeamScore(IList<ClubLadder> dataLadder, IList<NrlMatches> lastRound, string teamName)
        {
            if (DidWin(teamName, lastRound))
                return 0;

            var position = FindOponent(teamName, lastRound, dataLadder);
            return position <= 4 ? 10 : 0;
        }

        private static int FindOponent(string clubName, IList<NrlMatches> lastRound, IList<ClubLadder> ladder)
        {
            var lastMatch = lastRound.GetLastRound(clubName);

            return lastMatch.HomeTeam.NickName == clubName
                ? ladder.GetLadder(lastMatch.AwayTeam.NickName).Position
                : ladder.GetLadder(lastMatch.HomeTeam.NickName).Position;
        }
    }
}