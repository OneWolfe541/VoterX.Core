using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.Context;
using VoterX.NMStateElection;

namespace VoterX.Core.Elections
{
    public class NMElection
    {
        private int? _electionId;

        public Exception Error
        {
            get
            {
                return Lists.Error;
            }
        }

        public NMElection(int ElectionId)
        {
            _electionId = ElectionId;
            Create((int)_electionId);
        }

        public NMElection(int ElectionId, string connection)
        {
            _electionId = ElectionId;
            Create((int)_electionId, connection);
        }

        private SelfLoadingElectionLists _lists;
        public SelfLoadingElectionLists Lists
        {
            get
            {
                if (_lists == null && _electionId != null)
                {
                    Create((int)_electionId);
                }
                else if (_electionId == null)
                {
                    throw new Exception("Election Id not set.");
                }
                return _lists;
            }

            private set
            {
                _lists = value;
            }
        }

        internal void Create(int ElectionId)
        {
            if (_lists == null)
            {
                _lists = new SelfLoadingElectionLists(ElectionId);

                _electionId = ElectionId;
                _lists.PreloadAllLists(ElectionId);
            }
        }

        internal void Create(int ElectionId, string connection)
        {
            if (_lists == null)
            {
                _lists = new SelfLoadingElectionLists(ElectionId, connection);

                _electionId = ElectionId;
                _lists.PreloadAllLists(ElectionId);
            }
        }

        public bool Exists()
        {
            using (var context = new ElectionContext())
            {
                if (context != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ExistsAsync()
        {
            return await Task.Run(() =>
            {
                using (var context = new ElectionContext())
                {
                    if (context != null)
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        #region Logins
        public async Task<bool> SaveLogin(int PollId, int Computer)
        {
            return await UpdateLog(PollId, Computer, AuthTypes.Login);
        }

        public bool SaveLoginSync(int PollId, int Computer)
        {
            return UpdateLogSync(PollId, Computer, AuthTypes.Login);
        }

        public async Task<bool> SaveLogout(int PollId, int Computer)
        {
            return await UpdateLog(PollId, Computer, AuthTypes.Logout);
        }

        private async Task<bool> UpdateLog(int PollId, int Computer, AuthTypes LogType)
        {
            return false;
            //using (var context = new ElectionContext())
            //{
            //    if (context != null)
            //    {
            //        var lastLog = context.AvLocationDevices
            //            .Where(log =>
            //            log.PollId == PollId &&
            //            log.Computer == Computer
            //            ).FirstOrDefault();

            //        if (lastLog != null)
            //        {
            //            // Update record
            //            switch(LogType)
            //            {
            //                case AuthTypes.Login:
            //                    lastLog.LoggedIn = DateTime.Now;
            //                    break;
            //                case AuthTypes.Logout:
            //                    lastLog.LoggedOut = DateTime.Now;
            //                    break;
            //            }
            //        }
            //        else
            //        {
            //            // Create new record
            //            AvLocationDevice newLog = new AvLocationDevice();
            //            newLog.PollId = PollId;
            //            newLog.Computer = Computer;

            //            switch (LogType)
            //            {
            //                case AuthTypes.Login:
            //                    newLog.LoggedIn = DateTime.Now;
            //                    break;
            //                case AuthTypes.Logout:
            //                    newLog.LoggedOut = DateTime.Now;
            //                    break;
            //            }

            //            context.AvLocationDevices.Add(newLog);
            //        }

            //        try
            //        {
            //            await context.SaveChangesAsync();
            //            return true;
            //        }
            //        catch
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
        }

        private bool UpdateLogSync(int PollId, int Computer, AuthTypes LogType)
        {
            return false;
            //using (var context = new ElectionContext())
            //{
            //    if (context != null)
            //    {
            //        var lastLog = context.AvLocationDevices
            //            .Where(log =>
            //            log.PollId == PollId &&
            //            log.Computer == Computer
            //            ).FirstOrDefault();

            //        if (lastLog != null)
            //        {
            //            // Update record
            //            switch (LogType)
            //            {
            //                case AuthTypes.Login:
            //                    lastLog.LoggedIn = DateTime.Now;
            //                    break;
            //                case AuthTypes.Logout:
            //                    lastLog.LoggedOut = DateTime.Now;
            //                    break;
            //            }
            //        }
            //        else
            //        {
            //            // Create new record
            //            AvLocationDevice newLog = new AvLocationDevice();
            //            newLog.PollId = PollId;
            //            newLog.Computer = Computer;

            //            switch (LogType)
            //            {
            //                case AuthTypes.Login:
            //                    newLog.LoggedIn = DateTime.Now;
            //                    break;
            //                case AuthTypes.Logout:
            //                    newLog.LoggedOut = DateTime.Now;
            //                    break;
            //            }

            //            context.AvLocationDevices.Add(newLog);
            //        }

            //        try
            //        {
            //            context.SaveChanges();
            //            return true;
            //        }
            //        catch
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
        }

        #endregion

        #region Utilities
        public List<ElectionModel> LoadAllElections()
        {
            // Create disposable database context
            using (var context = new ElectionContext())
            {
                // Get election data from database
                var electionDataList = context.SosElections.ToList();

                if (electionDataList != null)
                {
                    List<ElectionModel> electionList = new List<ElectionModel>();

                    foreach (var electionData in electionDataList)
                    {
                        ElectionModel election = new ElectionModel();

                        // Copy data to election model
                        election.ElectionId = electionData.ElectionId;
                        election.CountyCode = electionData.CountyCode;
                        election.PollsOpenTime = electionData.PollsOpenTime;
                        election.PollsCloseTime = electionData.PollsCloseTime;
                        election.ElectionType = electionData.ElectionType;
                        election.CountyName = electionData.CountyName;
                        election.LastModified = electionData.LastModified;
                        election.ElectionName = electionData.ElectionName.Trim();
                        election.ElectionDate = electionData.ElectionDate;
                        election.AbsenteeBeginDate = electionData.AbsenteeBeginDate;
                        election.AbsenteeEndDate = electionData.AbsenteeEndDate;
                        election.UocavaBeginDate = electionData.UocavaBeginDate;
                        election.EarlyVotingBeginDate = electionData.EarlyVotingBeginDate;
                        election.EarlyVotingEndDate = electionData.EarlyVotingEndDate;
                        election.BookCloseDate = electionData.BookCloseDate;

                        electionList.Add(election);
                    }

                    return electionList;
                }
                else
                {
                    return null;
                }

            } // END USING

        } // END LOADALLELECTIONS
        #endregion
    }
}
