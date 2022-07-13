using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.ScanHistory;
using VoterX.Core.Extensions;

namespace VoterX.Core.Voters
{
    public static class NMVoterExtensions
    {
        /// <summary>
        /// Returns true if the log code is greater than 5.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool HasVoted(this NMVoter voter)
        {
            //bool result = false;

            //if (voter != null)
            //    if (voter.Data.LogCode != null)
            //        if (voter.Data.LogCode >= 5) return true;

            return voter.Data.HasVoted();
        }

        public static bool HasBeenIssued(this NMVoter voter)
        {
            bool result = false;

            if (voter != null)
                if (voter.Data.LogCode != null)
                    if (voter.Data.LogCode >= 6) result = true;

            return result;
        }

        /// <summary>
        /// Returns true if the log date is the same as today.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool VotedToday(this NMVoter voter)
        {
            //bool result = false;
            //if (voter != null)
            //    if (voter.Data.LogDate != null)
            //    {
            //        DateTime today = DateTime.Now.Date;
            //        DateTime votedOn = ((DateTime)voter.Data.LogDate).Date;
            //        if (today == votedOn) result = true;
            //    }
            return voter.Data.VotedToday();
        }

        /// <summary>
        /// Returns true if the log date is the same as today and the given site matches what is in the voted record.
        /// </summary>
        /// <param name="voter"></param>
        /// <param name="currentSiteID"></param>
        /// <returns></returns>
        public static bool VotedHereToday(this NMVoter voter, int currentSiteID)
        {
            //bool result = false;
            //if (voter != null)
            //    if (voter.Data.LogDate != null && voter.Data.PollID != null)
            //    {
            //        DateTime today = DateTime.Now.Date;
            //        DateTime votedOn = ((DateTime)voter.Data.LogDate).Date;
            //        int voterVotedAtID = (int)voter.Data.PollID;
            //        if (today == votedOn && currentSiteID == voterVotedAtID) result = true;
            //    }
            return voter.Data.VotedHereToday(currentSiteID);
        }

        public static bool WrongOrFledVoter(this NMVoter voter)
        {
            //bool result = false;
            //if (voter != null)
            //    if (voter.Data.WrongVoter == true || voter.Data.FledVoter == true)
            //    {
            //        result = true;
            //    }
            return voter.Data.WrongOrFledVoter();
        }

        public static bool IsEligible(this NMVoter voter)
        {
            //bool result = false;
            //if (voter.Data.BallotStyleID != null)
            //    result = true;
            return voter.Data.IsEligible();
        }

        public static bool IsValidPollId(this NMVoter voter, int pollId)
        {
            bool result = false;
            if (voter.ValidatePrecinctPoll(pollId) != null)
                result = true;
            return result;
        }

        public static bool? IdRequired(this NMVoter voter)
        {
            return voter.Data.IDRequired??false;
        }

        /// <summary>
        /// Returns true if the log code is equal to 4.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool HasRejectedApplication(this NMVoter voter)
        {
            //bool result = false;

            //if (voter != null)
            //    if (voter.Data.LogCode != null)
            //        if (voter.Data.LogCode == 4) return true;

            return voter.Data.HasRejectedApplication();
        }

        public static bool WasRemoved(this NMVoter voter)
        {
            //bool result = false;

            //if (voter != null)
            //    if (voter.Data.LogCode != null)
            //        if (voter.Data.LogCode == 17) return true;

            return voter.Data.WasRemoved();
        }

        public static bool HasDeliveryAddress(this NMVoter voter)
        {
            bool result = false;

            if(voter.Data.LogCode >= 3)
            {
                if (
                    !voter.Data.DeliveryAddress1.IsNullOrSpace()
                    ||
                    !voter.Data.DeliveryAddress2.IsNullOrSpace()
                    ||
                    !voter.Data.DeliveryCity.IsNullOrSpace()
                    ||
                    !voter.Data.DeliveryState.IsNullOrSpace()
                    ||
                    !voter.Data.DeliveryZip.IsNullOrSpace()
                    ||
                    !voter.Data.DeliveryCountry.IsNullOrSpace()
                    )
                {
                    result = true;
                }
            }
            //else
            //{
            //   // Defaults to false
            //}

            return result;
        }

        public static string CheckForErrors(this ObservableCollection<NMVoter> voterList)
        {
            if (voterList != null && voterList.Count() == 1)
            {
                foreach (NMVoter item in voterList)
                {
                    if (item.Error != null)
                    {
                        return item.Error.Message;
                    }
                }
            }
            return null;
        }

        public static void RemoveFromScanHistory(this NMVoter voter)
        {
            using (var factory = new ScanHistoryFactory())
            {
                factory.RemoveVoter(voter);
            }
        }

        public static void RemoveFromScanHistory(this NMVoter voter, string connection)
        {
            using (var factory = new ScanHistoryFactory(connection))
            {
                factory.RemoveVoter(voter);
            }
        }
    }
}
