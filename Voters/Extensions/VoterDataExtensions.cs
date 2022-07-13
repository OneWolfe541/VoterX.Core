using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.Extensions;
//using VoterX.Core.Voters.Models;

namespace VoterX.Core.Voters
{
    /// <summary>
    /// Provides additional functionality to the Voter model for internal use only.
    /// </summary>
    internal static class VoterDataExtensions
    {
        internal static IQueryable<T> IDEquals<T>(this IQueryable<T> queryable, string strID) where T : VoterDataModel
        {
            if (strID.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.VoterID == strID);
        }

        internal static IQueryable<T> LastNameStartsWith<T>(this IQueryable<T> queryable, string strLastName) where T : VoterDataModel
        {
            if (strLastName.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.LastName.ToUpper().StartsWith(strLastName.ToUpper()));
        }

        internal static IQueryable<T> FirstNameStartsWith<T>(this IQueryable<T> queryable, string strFirstName) where T : VoterDataModel
        {
            if (strFirstName.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.FirstName.ToUpper().StartsWith(strFirstName.ToUpper()));
        }

        internal static IQueryable<T> BirthDateContains<T>(this IQueryable<T> queryable, string strDate) where T : VoterDataModel
        {
            if (strDate.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.DOBSearch.Contains(strDate));
        }

        internal static IQueryable<T> BirthDateEquals<T>(this IQueryable<T> queryable, string strDate) where T : VoterDataModel
        {
            if (strDate.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.DOBSearch == strDate);
        }

        internal static IQueryable<T> BirthYearEquals<T>(this IQueryable<T> queryable, string strYear) where T : VoterDataModel
        {
            if (strYear.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.DOBYear == strYear);
        }

        internal static IQueryable<T> BirthYearContains<T>(this IQueryable<T> queryable, string strYear) where T : VoterDataModel
        {
            if (strYear.IsNullOrEmpty() || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.DOBYear.Contains(strYear));
        }

        // https://stackoverflow.com/questions/5624614/get-a-list-of-elements-by-their-id-in-entity-framework?lq=1
        internal static IQueryable<T> FromList<T>(this IQueryable<T> queryable, List<string> voterIdList) where T : VoterDataModel
        {
            if (voterIdList == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => voterIdList.Contains(arg.VoterID));
        }

        internal static IQueryable<T> AtPollSite<T>(this IQueryable<T> queryable, int? pollID) where T : VoterDataModel
        {
            if (pollID == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.PollID == pollID);
        }

        internal static IQueryable<T> FederalBallot<T>(this IQueryable<T> queryable, int? pollID) where T : VoterDataModel
        {
            if (pollID == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.PollID == pollID && arg.LogCode == 5);
        }

        internal static IQueryable<T> OnMachine<T>(this IQueryable<T> queryable, int? computerID) where T : VoterDataModel
        {
            if (computerID == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.ComputerID == computerID);
        }

        internal static IQueryable<T> WithLogCode<T>(this IQueryable<T> queryable, int? logCode) where T : VoterDataModel
        {
            if (logCode == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.LogCode == logCode);
        }

        internal static IQueryable<T> LogCodeLessthan<T>(this IQueryable<T> queryable, int? logCode) where T : VoterDataModel
        {
            if (logCode == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.LogCode < logCode);
        }

        internal static IQueryable<T> WithBallotStyle<T>(this IQueryable<T> queryable, int? ballotStyleID) where T : VoterDataModel
        {
            if (ballotStyleID == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.BallotStyleID == ballotStyleID);
        }

        internal static IQueryable<T> ListContains<T>(this IQueryable<T> queryable, List<string> idList) where T : VoterDataModel
        {
            if (idList == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => idList.Contains(arg.VoterID));
        }

        internal static IQueryable<T> IdRequired<T>(this IQueryable<T> queryable, bool? required) where T : VoterDataModel
        {
            if (required == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.IDRequired == required);
        }

        internal static IQueryable<T> FromBatch<T>(this IQueryable<T> queryable, Guid? batchId) where T : VoterDataModel
        {
            if (batchId == null || queryable == null)
                return queryable.Where(arg => arg.BatchID == null);
            else
                return queryable.Where(arg => arg.BatchID == batchId);
        }
    }

    /// <summary>
    /// Provides additional functionality to the VoterDataModel for public use.
    /// </summary>
    public static class VoterExtensions
    {
        /// <summary>
        /// Returns true if the log code is greater than 5.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool HasVoted(this VoterDataModel voter)
        {
            bool result = false;

            if (voter != null)
                if (voter.LogCode != null)
                    if (voter.LogCode >= 5) return true;

            return result;
        }

        /// <summary>
        /// Returns true if the log date is the same as today.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool VotedToday(this VoterDataModel voter)
        {
            bool result = false;
            if (voter != null)
                if (voter.VotedDate != null)
                {
                    DateTime today = DateTime.Now.Date;
                    DateTime votedOn = ((DateTime)voter.VotedDate).Date;
                    if (today == votedOn) result = true;
                }
            return result;
        }

        /// <summary>
        /// Returns true if the log date is the same as today and the given site matches what is in the voted record.
        /// </summary>
        /// <param name="voter"></param>
        /// <param name="currentSiteID"></param>
        /// <returns></returns>
        public static bool VotedHereToday(this VoterDataModel voter, int currentSiteID)
        {
            bool result = false;
            if (voter != null)
                if (voter.VotedDate != null && voter.PollID != null)
                {
                    DateTime today = DateTime.Now.Date;
                    DateTime votedOn = ((DateTime)voter.VotedDate).Date;
                    int voterVotedAtID = (int)voter.PollID;
                    if (today == votedOn && currentSiteID == voterVotedAtID) result = true;
                }
            return result;
        }

        public static bool WrongOrFledVoter(this VoterDataModel voter)
        {
            bool result = false;
            if (voter != null)
                if (voter.WrongVoter == true || voter.FledVoter == true)
                {
                    result = true;
                }
            return result;
        }

        public static bool IsEligible(this VoterDataModel voter)
        {
            bool result = false;
            if (voter.BallotStyleID != null)
                result = true;
            return result;
        }

        /// <summary>
        /// Returns true if the log code is equal to 4.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool HasRejectedApplication(this VoterDataModel voter)
        {
            bool result = false;

            if (voter != null)
                if (voter.LogCode != null)
                    if (voter.LogCode == 4) return true;

            return result;
        }

        public static string CheckForError(this VoterDataModel voter)
        {
            if (voter.Error != null)
            {
                return voter.Error.Message;
            }
            return null;
        }

        public static string CheckForErrors(this ObservableCollection<VoterDataModel> voterList)
        {
            if (voterList != null && voterList.Count() == 1)
            {
                foreach (VoterDataModel item in voterList)
                {
                    if (item.Error != null)
                    {
                        return item.Error.Message;
                    }
                }
            }
            return null;
        }

        public static bool WasRemoved(this VoterDataModel voter)
        {
            bool result = false;

            if (voter != null)
                if (voter.LogCode != null)
                    if (voter.LogCode == 17) return true;

            return result;
        }
    }
}
