using System.Collections.Generic;
using footytips.Models;

namespace footytips.Rules
{
    public interface IRule
    {
        Score GetScoreClass(NrlMatches game, IList<ClubLadder> dataLadder, IList<NrlMatches> lastRound);
    }
}