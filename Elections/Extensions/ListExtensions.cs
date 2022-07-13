using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterX.Core.Elections
{
    public static class ListExtensions
    {
        // Strip out the Precinct Part Ids from the list in order to get only the Unique ballot styles
        public static List<BallotStyleModel> DistinctBallots(this List<BallotStyleModel> BallotList)
        {
            var ballots = BallotList.Select(bs => new
            {
                bs.BallotStyleId,
                bs.ElectionId,
                bs.CountyCode,
                bs.BallotStyleName,
                bs.BallotStyleFileName,
                bs.Party,
                bs.LastModified,
                bs.ModificationType
            }).Distinct().ToList();

            return ballots.Select(bs => new BallotStyleModel
            {
                BallotStyleId = bs.BallotStyleId,
                ElectionId = bs.ElectionId,
                CountyCode = bs.CountyCode,
                BallotStyleName = bs.BallotStyleName,
                BallotStyleFileName = bs.BallotStyleFileName,
                Party = bs.Party,
                LastModified = bs.LastModified,
                ModificationType = bs.ModificationType
            }).ToList();
        }        
    }
}
