using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public class LastGameBigLoss : RuleBase
    {
        protected override string RuleName => "Last Game Big Loss";

        protected override (int, int) GetScore(NrlMatches game, IList<ClubLadder> dataLadder,
            IList<NrlMatches> lastRound)
        {
            int h = 0, a = 0;

            if (!DidWin(game.HomeTeam.NickName, lastRound) &&
                GetGameScore(game.HomeTeam.NickName, lastRound) < -15) a = 10;

            if (!DidWin(game.AwayTeam.NickName, lastRound) &&
                GetGameScore(game.AwayTeam.NickName, lastRound) < -15) h = 10;

            return (h, a);
        }

        private static int GetGameScore(string teamName, IList<NrlMatches> lastRound)
        {
            var lastGame = lastRound.GetLastRound(teamName);

            if (lastGame.HomeTeam.Score > lastGame.AwayTeam.Score)
                return lastGame.HomeTeam.Score - lastGame.AwayTeam.Score;
            return lastGame.AwayTeam.Score - lastGame.HomeTeam.Score;
        }
    }
}