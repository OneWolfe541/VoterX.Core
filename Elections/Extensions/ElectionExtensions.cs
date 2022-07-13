using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterX.Core.Elections
{
    public static class ElectionExtensions
    {
        public static bool AbsenteeHasStarted(this NMElection election)
        {
            bool result = false;

            // Test for empty election object
            if(election != null)
            {
                // Test for empty election data
                if(election.Lists.Election != null)

                    // Test for empty absentee date 
                    if(election.Lists.Election.AbsenteeBeginDate != null

                        // and if absentee date was on or before today
                        && election.Lists.Election.AbsenteeBeginDate >= DateTime.Now)
                    {
                        result = true;
                    }
            }

            //// Possibly a simpler method for the same equation
            //result = (election != null) &&
            //         (election.Lists.Election != null) &&
            //         (election.Lists.Election.AbsenteeBeginDate != null) &&
            //         (election.Lists.Election.AbsenteeBeginDate <= DateTime.Now);

            //// Or the Try/Catch method would eliminate all the null checks
            //try
            //{
            //    result = (election.Lists.Election.AbsenteeBeginDate <= DateTime.Now);
            //}
            //catch
            //{
            //    result = false;
            //}

            return result;
        }

        public static bool BallotProcessingWindow(this NMElection election)
        {
            bool result = false;

            // Test for empty election object
            if (election != null)
            {
                // Test for empty election data
                if (election.Lists.Election != null)

                    // Test for empty absentee date 
                    if (election.Lists.Election.AbsenteeBeginDate != null

                        // and if absentee date (minus 10) was on or after today
                        && election.Lists.Election.AbsenteeBeginDate.Value.AddDays(-10) <= DateTime.Now)
                    {
                        result = true;
                    }
            }

            return result;
        }
    }
}
