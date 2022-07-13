using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.AbsenteeBoard;
using VoterX.Core.Extensions;
using VoterX.Core.ScanHistory;
using VoterX.Core.Utilities;

namespace VoterX.Core.Voters
{
    // DUPLICATE METHODS CREATED TO HANDLE LISTS AND OBSERVABLECOLLECTIONS
    public static class NMVoterListExtensions
    {
        // Issue ballots and accept application
        public static void IssueBallots(this List<NMVoter> VoterList)
        {
            foreach (var voter in VoterList)
            {
                voter.IssueBallot();
            }
        }
        public static void IssueBallots(this ObservableCollection<NMVoter> VoterList)
        {
            (new List<NMVoter>(VoterList)).IssueBallots();
        }

        // Accpet absentee application
        public static void AcceptApplications(this List<NMVoter> VoterList)
        {
            DateTime accepted = DateTime.Now;
            VoterList.AcceptApplications(accepted);
        }
        public static void AcceptApplications(this List<NMVoter> VoterList, DateTime accepted)
        {
            foreach (var voter in VoterList)
            {
                voter.Data.ApplicationAccepted = accepted;
                voter.AcceptApplication();
            }
        }
        public static void AcceptApplications(this ObservableCollection<NMVoter> VoterList)
        {
            DateTime accepted = DateTime.Now;
            (new List<NMVoter>(VoterList)).AcceptApplications();
        }
        public static void AcceptApplications(this ObservableCollection<NMVoter> VoterList, DateTime accepted)
        {
            (new List<NMVoter>(VoterList)).AcceptApplications(accepted);
        }

        // Return ballots and mark with a specific log code
        public static void ReturnBallots(this List<NMVoter> VoterList, int LogCode)
        {
            foreach (var voter in VoterList)
            {
                voter.ReturnBallot(LogCode);
            }
        }
        public static void ReturnBallots(this List<NMVoter> VoterList, LogCodes LogCode)
        {
            foreach (var voter in VoterList)
            {
                voter.ReturnBallot(LogCode);
            }
        }
        public static void ReturnBallots(this ObservableCollection<NMVoter> VoterList, int LogCode)
        {
            (new List<NMVoter>(VoterList)).ReturnBallots(LogCode);
        }
        public static void ReturnBallots(this ObservableCollection<NMVoter> VoterList, LogCodes LogCode)
        {
            (new List<NMVoter>(VoterList)).ReturnBallots(LogCode);
        }

        // Reject ballots and mark with rejection code
        public static void RejectBallots(this List<NMVoter> VoterList, string rejectionCode)
        {
            foreach (var voter in VoterList)
            {
                voter.RejectBallot(rejectionCode);
            }
        }
        public static void RejectBallots(this ObservableCollection<NMVoter> VoterList, string rejectionCode)
        {
            (new List<NMVoter>(VoterList)).RejectBallots(rejectionCode);
        }

        // Manually change the voters log code status
        public static void ChangeStatus(this List<NMVoter> VoterList, DateTime ReceivedDate, int LogCode, string ChangeReason)
        {
            foreach (var voter in VoterList)
            {
                voter.ChangeLogCode(ReceivedDate, LogCode, ChangeReason, null);
            }
        }
        public static void ChangeStatus(this List<NMVoter> VoterList, int LogCode, string ChangeReason)
        {
            VoterList.ChangeStatus(DateTime.Now, LogCode, ChangeReason);
        }
        public static void ChangeStatus(this ObservableCollection<NMVoter> VoterList, DateTime ReceivedDate, int LogCode, string ChangeReason)
        {
            (new List<NMVoter>(VoterList)).ChangeStatus(LogCode, ChangeReason);
        }
        public static void ChangeStatus(this ObservableCollection<NMVoter> VoterList, int LogCode, string ChangeReason)
        {
            (new List<NMVoter>(VoterList)).ChangeStatus(DateTime.Now, LogCode, ChangeReason);
        }

        public static void ChangeStatus(this List<NMVoter> VoterList, DateTime ReceivedDate, int LogCode, string ChangeReason, int? RejectedReasonId)
        {
            foreach (var voter in VoterList)
            {
                voter.ChangeLogCode(ReceivedDate, LogCode, ChangeReason, RejectedReasonId);
            }
        }
        public static void ChangeStatus(this ObservableCollection<NMVoter> VoterList, DateTime ReceivedDate, int LogCode, string ChangeReason, int? RejectedReasonId)
        {
            (new List<NMVoter>(VoterList)).ChangeStatus(ReceivedDate, LogCode, ChangeReason, RejectedReasonId);
        }

        // https://stackoverflow.com/questions/1565289/union-two-observablecollection-lists
        public static List<NMVoter> CombineLists(this List<NMVoter> originalList, List<NMVoter> listToAdd)
        {
            List<NMVoter> list = new List<NMVoter>();
            foreach (var v in listToAdd.Union(originalList)) list.Add(v);
            return list;
        }

        public static ObservableCollection<NMVoter> CombineLists(this ObservableCollection<NMVoter> originalList, ObservableCollection<NMVoter> listToAdd)
        {
            ObservableCollection<NMVoter> list = new ObservableCollection<NMVoter>();
            foreach (var v in listToAdd.Union(originalList)) list.Add(v);
            return list;
        }

        // Updating the original list does not work
        //public static void CombineLists(this ObservableCollection<NMVoter> originalList, ObservableCollection<NMVoter> listToAdd)
        //{
        //    ObservableCollection<NMVoter> list = new ObservableCollection<NMVoter>();
        //    foreach (var v in listToAdd.Union(originalList)) list.Add(v);
        //    originalList = list;
        //}

        // Add local user data to the voter list
        public static void Localize(this List<NMVoter> VoterList, int election, int poll, int? computer, int? user)
        {
            foreach (var voter in VoterList)
            {
                voter.Localize(election, poll, computer, user);
            }
        }

        public static void Localize(this ObservableCollection<NMVoter> VoterList, int election, int poll, int? computer, int? user)
        {
            (new List<NMVoter>(VoterList)).Localize(election, poll, computer, user);
            //foreach (var voter in VoterList)
            //{
            //    voter.Localize(election, poll, computer, 0);
            //}
        }

        // Update location data (MUST BE USED AFTER LOCAL DATA IS SET)
        public static void UpdateLocation(this List<NMVoter> VoterList)
        {
            foreach (var voter in VoterList)
            {
                voter.UpdateLocation();
            }
        }
        public static void UpdateLocation(this ObservableCollection<NMVoter> VoterList)
        {
            (new List<NMVoter>(VoterList)).UpdateLocation();
        }

        public static void UpdateUser(this List<NMVoter> VoterList, int? UserId)
        {
            foreach (var voter in VoterList)
            {
                voter.UpdateUser(UserId);
            }
        }
        public static void UpdateUser(this ObservableCollection<NMVoter> VoterList, int? UserId)
        {
            (new List<NMVoter>(VoterList)).UpdateUser(UserId);
        }

        public static void UpdateForBatch(this ObservableCollection<NMVoter> VoterList, int? UserId)
        {
            (new List<NMVoter>(VoterList)).UpdateForBatch(UserId);
        }
        public static void UpdateForBatch(this List<NMVoter> VoterList, int? UserId)
        {
            foreach (var voter in VoterList)
            {
                voter.UpdateForBatch(UserId);
            }
        }

        public static void UpdateForBatch(this ObservableCollection<NMVoter> VoterList, Guid? batchId)
        {
            (new List<NMVoter>(VoterList)).UpdateForBatch(batchId);
        }
        public static void UpdateForBatch(this List<NMVoter> VoterList, Guid? batchId)
        {
            foreach (var voter in VoterList)
            {
                voter.UpdateForBatch(batchId);
            }
        }

        // Order Voter List options
        public static List<NMVoter> OrderByLastName(this List<NMVoter> VoterList)
        {
            return VoterList
                .OrderBy(o => o.Data.LastName)
                .ToList();
        }
        public static ObservableCollection<NMVoter> OrderByLastName(this ObservableCollection<NMVoter> VoterList)
        {
            return new ObservableCollection<NMVoter>((new List<NMVoter>(VoterList)).OrderByLastName());
        }

        public static List<NMVoter> OrderByFullName(this List<NMVoter> VoterList)
        {
            return VoterList
                .OrderBy(o => o.Data.LastName)
                .ThenBy(o => o.Data.FirstName)
                .ThenBy(o => o.Data.MiddleName)
                //.ThenBy(o => o.Data.DOBYear)
                .ToList();
        }
        public static ObservableCollection<NMVoter> OrderByFullName(this ObservableCollection<NMVoter> VoterList)
        {
            return new ObservableCollection<NMVoter>((new List<NMVoter>(VoterList)).OrderByFullName());
        }

        #region ScanHistory
        /// <summary>
        /// Create a Scan History session and add the voters to it.
        /// </summary>
        /// <param name="VoterList"></param>
        /// <param name="pollId"></param>
        /// <param name="computerNumber"></param>
        /// <param name="date"></param>
        public static void AddToScanHistory(this List<NMVoter> VoterList, int? poll, int computer, DateTime date, string connection)
        {
            using (var factory = new ScanHistoryFactory(connection))
            {
                // Create a new session
                if (poll == null) poll = 0;
                var session = factory.Create((int)poll, computer, date);

                // Add voters to the new session
                session.AddVoters(VoterList);
            }
        }

        public static void AddToScanHistory(this ObservableCollection<NMVoter> VoterList, int? poll, int computer, DateTime date, string connection)
        {
            (new List<NMVoter>(VoterList)).AddToScanHistory(poll, computer, date, connection);
        }

        #endregion

        #region AbsenteeBoard
        /// <summary>
        /// Create a Scan History session and add the voters to it.
        /// </summary>
        /// <param name="VoterList"></param>
        /// <param name="pollId"></param>
        /// <param name="computerNumber"></param>
        /// <param name="date"></param>
        public static void AddToAbsenteeBoard(this List<NMVoter> VoterList, int? poll, int computer, DateTime date, string loc1, string loc2, string connection)
        {
            using (var factory = new AbsenteeBoardFactory(connection))
            {
                // Create a new session
                if (poll == null) poll = 0;
                List<AbsenteeBoardModel> boardList = ConvertToAbsenteeBoard(VoterList, poll, computer, date, loc1, loc2);
                factory.InsertBoardVoters(boardList);
                boardList = null;
            }
        }

        public static void AddToAbsenteeBoard(this ObservableCollection<NMVoter> VoterList, int? poll, int computer, DateTime date, string loc1, string loc2, string connection)
        {
            (new List<NMVoter>(VoterList)).AddToAbsenteeBoard(poll, computer, date, loc1, loc2, connection);
        }

        public static List<AbsenteeBoardModel> ConvertToAbsenteeBoard(List<NMVoter> VoterList, int? poll, int computer, DateTime date, string loc1, string loc2)
        {
            if (VoterList != null)
            {
                List<AbsenteeBoardModel> boardList = new List<AbsenteeBoardModel>();

                foreach (var voter in VoterList)
                {
                    AbsenteeBoardModel absenteeBoardVoter = new AbsenteeBoardModel
                    {
                        VoterId = voter.Data.VoterID.ToInt(),
                        ScanDate = date,
                        PollId = poll.Value,
                        Computer = computer,
                        Location1 = loc1,
                        Location2 = loc2,
                        LogCode = voter.Data.LogCode.Value
                    };

                    boardList.Add(absenteeBoardVoter);

                    absenteeBoardVoter = null;
                }

                return boardList;
            }
            else
            {
                return null;
            }
            
        }

        //Set IMB numbers
        public static bool GenerateIntelligentBarcodes(this List<NMVoter> VoterList, IntelligentBarcodeModel imb)
        {
            bool result = true;

            foreach (var voter in VoterList)
            {
                result = result && voter.GenerateIntelligentBarcodes(imb);
            }

            return result;
        }
        public static bool GenerateIntelligentBarcodes(this ObservableCollection<NMVoter> VoterList, IntelligentBarcodeModel imb)
        {
            return (new List<NMVoter>(VoterList)).GenerateIntelligentBarcodes(imb);
        }

        #endregion
    }
}
