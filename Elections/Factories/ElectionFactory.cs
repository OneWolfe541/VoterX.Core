using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using VoterX.Core.Context;
using VoterX.NMStateElection;
using System.Data.SqlClient;
using System.Data;

namespace VoterX.Core.Elections
{
    public class ElectionFactory : IDisposable
    {        
        //private int? _electionId;

        private bool disposed;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public ElectionFactory()
        {

        }

        //public ElectionFactory(string connection)
        //{

        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
            }
            disposed = true;
        }

        public bool Exists()
        {
            using (var context = new ElectionContext())
            {
                try
                {
                    context.Database.Connection.Open();
                    context.Database.Connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    var error = ex;
                    //throw ex;
                    return false;
                }                
            }
        }

        public async Task<bool> ExistsAsync()
        {
            return await Task.Run(() =>
            {
                using (var context = new ElectionContext())
                {
                    try
                    {
                        context.Database.Connection.Open();
                        context.Database.Connection.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                        return false;
                    }
                }
            });
        }

        public async Task<bool> ExistsAsync(string connection)
        {
            return await Task.Run(() =>
            {
                using (var context = new ElectionContext(connection))
                {
                    try
                    {
                        context.Database.Connection.Open();
                        context.Database.Connection.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                        return false;
                    }
                }
            });
        }

        public NMElection Create(int ElectionId)
        {
            return new NMElection(ElectionId);
        }

        public NMElection Create(int ElectionId, string connection)
        {
            return new NMElection(ElectionId, connection);
        }

        //private SelfLoadingElectionLists _lists;
        //public SelfLoadingElectionLists Lists
        //{
        //    get
        //    {
        //        if (_lists == null && _electionId != null)
        //        {
        //            _lists.Create((int)_electionId);
        //        }
        //        else if (_electionId == null)
        //        {
        //            throw new Exception("Election Id not set.");
        //        }
        //        return _lists;
        //    }

        //    private set
        //    {
        //        _lists = value;
        //    }
        //}

        //public void Create(int ElectionId)
        //{
        //    if (_lists == null)
        //    {
        //        _lists = new ElectionListsModel();

        //        _electionId = ElectionId;
        //        LoadElectionData(_electionId);

        //        LoadApplicationRejectedReasons();

        //        LoadBallotStyles(_electionId);

        //        LoadJurisdictions();

        //        LoadLocations();

        //        LoadLogCodes();

        //        LoadParties();

        //        LoadProvisionalReasons();

        //        LoadSpoiledReasons();
        //    }
        //}

        //public void Load(int ElectionId)
        //{
        //    Create(ElectionId);
        //}

        //private void LoadElectionData(int? electionId)
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get election data from database
        //        var electionData = context.SosElections.Where(elc => elc.ElectionId == electionId).FirstOrDefault();

        //        if (electionData != null)
        //        {
        //            // Load data into election
        //            _lists.Election.ElectionId = electionData.ElectionId;
        //            _lists.Election.CountyCode = electionData.CountyCode;
        //            _lists.Election.PollsOpenTime = electionData.PollsOpenTime;
        //            _lists.Election.PollsCloseTime = electionData.PollsCloseTime;
        //            _lists.Election.ElectionType = electionData.ElectionType;
        //            _lists.Election.CountyName = electionData.CountyName;
        //            _lists.Election.LastModified = electionData.LastModified;
        //            _lists.Election.ElectionName = electionData.ElectionName.Trim();
        //            _lists.Election.ElectionDate = electionData.ElectionDate;
        //            _lists.Election.AbsenteeBeginDate = electionData.AbsenteeBeginDate;
        //            _lists.Election.AbsenteeEndDate = electionData.AbsenteeEndDate;
        //            _lists.Election.UocavaBeginDate = electionData.UocavaBeginDate;
        //            _lists.Election.EarlyVotingBeginDate = electionData.EarlyVotingBeginDate;
        //            _lists.Election.EarlyVotingEndDate = electionData.EarlyVotingEndDate;
        //            _lists.Election.BookCloseDate = electionData.BookCloseDate;
        //        }
        //        else
        //        {
        //            _lists.Election = null;
        //        }

        //    } // END USING

        //} // END LOADELECTIONDATA

        //private void LoadApplicationRejectedReasons()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var rejectedReasons = context.RejectedReasons.ToList();

        //        if (rejectedReasons != null)
        //        {
        //            // Create temp list
        //            var applicationRejectedReasonList = new List<ApplicationRejectedReasonModel>();

        //            // Copy fields to temp list
        //            foreach (var reason in rejectedReasons)
        //            {
        //                ApplicationRejectedReasonModel newRejectedReason = new ApplicationRejectedReasonModel
        //                {
        //                    RejectedReasonId = reason.RejectedReasonId,
        //                    RejectedReasonDescription = reason.RejectedReason,
        //                    ServiseCode = reason.ServisCode,
        //                    VoterXCode = reason.VoterXCode
        //                };

        //                applicationRejectedReasonList.Add(newRejectedReason);
        //            }

        //            // Add temp list to election
        //            _lists.ApplicationRejectedReasons = applicationRejectedReasonList;

        //            // Clear temp list
        //            applicationRejectedReasonList = null;
        //        }

        //    } // END USING

        //} // END LOAD APPLICATIONREJECTEDREASON

        //private void LoadBallotStyles(int? ElectionId)
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var ballotStyles = context.SosBallotStyles.ToList();

        //        if (ballotStyles != null)
        //        {
        //            // Create temp list
        //            var ballotStylesList = new List<BallotStyleModel>();

        //            // Copy fields to temp list
        //            foreach (var style in ballotStyles)
        //            {
        //                BallotStyleModel newRejectedReason = new BallotStyleModel
        //                {
        //                    BallotStyleId = style.BallotStyleId,
        //                    ElectionId = style.ElectionId,
        //                    CountyCode = style.CountyCode,
        //                    BallotStyleName = style.BallotStyleName,
        //                    BallotStyleFileName = style.BallotStyleFileName,
        //                    Party = style.Party,
        //                    LastModified = style.LastModified,
        //                    ModificationType = style.ModificationType
        //                };

        //                ballotStylesList.Add(newRejectedReason);
        //            }

        //            // Add temp list to election
        //            _lists.BallotStyles = ballotStylesList;

        //            // Clear temp list
        //            ballotStylesList = null;
        //        }

        //    } // END USING

        //} // END LOAD BALLOTSTYLES

        //private void LoadJurisdictions()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var jurisdictions = context.SosJurisdictions.ToList();

        //        if (jurisdictions != null)
        //        {
        //            // Create temp list
        //            var jurisdictionList = new List<JurisdictionModel>();

        //            // Copy fields to temp list
        //            foreach (var juryItem in jurisdictions)
        //            {
        //                JurisdictionModel newJurisdiction = new JurisdictionModel
        //                {
        //                    JurisdictionId = juryItem.JurisdictionId,
        //                    JurisdictionType = juryItem.JurisdictionType,
        //                    JurisdictionName = juryItem.JurisdictionName,
        //                    CountyCode = juryItem.CountyCode,
        //                    LastModified = juryItem.LastModified,
        //                    ModificationType = juryItem.ModificationType
        //                };

        //                jurisdictionList.Add(newJurisdiction);
        //            }

        //            // Add temp list to election
        //            _lists.Jurisdictions = jurisdictionList;

        //            // Clear temp list
        //            jurisdictionList = null;
        //        }

        //    } // END USING

        //} // END LOAD JURISDICTIONS

        //private void LoadLocations()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var locations = context.SosLocations.ToList();

        //        if (locations != null)
        //        {
        //            // Create temp list
        //            var locationsList = new List<LocationModel>();

        //            // Copy fields to temp list
        //            foreach (var locationItem in locations)
        //            {
        //                LocationModel newLocation = new LocationModel
        //                {
        //                    PollId = locationItem.PollId,
        //                    ElectionId = locationItem.ElectionId,
        //                    CountyCode = locationItem.CountyCode,
        //                    PlaceName = locationItem.PlaceName,
        //                    Address = locationItem.Address,
        //                    City = locationItem.City,
        //                    Zip = locationItem.Zip,
        //                    FacilityContact = locationItem.FacilityContact,
        //                    FacilityPhone = locationItem.FacilityPhone,
        //                    LastModified = locationItem.LastModified,
        //                    ModificationType = locationItem.ModificationType
        //                };

        //                locationsList.Add(newLocation);
        //            }

        //            // Add temp list to election
        //            _lists.Locations = locationsList;

        //            // Clear temp list
        //            locationsList = null;
        //        }

        //    } // END USING

        //} // END LOAD LOCATIONS

        //private void LoadLogCodes()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var logcodes = context.AvLogCodes.ToList();

        //        if (logcodes != null)
        //        {
        //            // Create temp list
        //            var logCodesList = new List<LogCodeModel>();

        //            // Copy fields to temp list
        //            foreach (var logItem in logcodes)
        //            {
        //                LogCodeModel newLogCode = new LogCodeModel
        //                {
        //                    LogCodeId = logItem.LogCodeId,
        //                    LogDescription = logItem.LogDescription,
        //                    ServisCode = logItem.ServisCode,
        //                };

        //                logCodesList.Add(newLogCode);
        //            }

        //            // Add temp list to election
        //            _lists.LogCodes = logCodesList;

        //            // Clear temp list
        //            logCodesList = null;
        //        }

        //    } // END USING

        //} // END LOAD LOGCODES

        //private void LoadParties()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var parties = context.SosVoters.Select(p => p.Party).Distinct().ToList();

        //        if (parties != null)
        //        {
        //            // Create temp list
        //            var partyList = new List<PartyModel>();

        //            // Copy fields to temp list
        //            foreach (var partyItem in parties)
        //            {
        //                PartyModel newParty = new PartyModel
        //                {
        //                    PartyCode = partyItem
        //                };

        //                partyList.Add(newParty);
        //            }

        //            // Add temp list to election
        //            _lists.Partys = partyList;

        //            // Clear temp list
        //            partyList = null;
        //        }

        //    } // END USING

        //} // END LOAD PARTIES

        //private void LoadPollWorkers()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var pollworkers = context.SosPollWorkers.ToList();

        //        if (pollworkers != null)
        //        {
        //            // Create temp list
        //            var pollWorkerList = new List<PollWorkerModel>();

        //            // Copy fields to temp list
        //            foreach (var workerItem in pollworkers)
        //            {
        //                PollWorkerModel newPollWorker = new PollWorkerModel
        //                {
        //                    VoterId = workerItem.VoterId,
        //                    ElectionId = workerItem.ElectionId,
        //                    CountyCode = workerItem.CountyCode,
        //                    FirstName = workerItem.FirstName,
        //                    MiddleName = workerItem.MiddleName,
        //                    LastName = workerItem.LastName,
        //                    Suffix = workerItem.Suffix,
        //                    Phone = workerItem.Phone,
        //                    Party = workerItem.Party,
        //                    PollId = workerItem.PollId,
        //                    Address = workerItem.Address,
        //                    City = workerItem.City,
        //                    State = workerItem.State,
        //                    Zip = workerItem.Zip,
        //                    LastModified = workerItem.LastModified,
        //                    ModificationType = workerItem.ModificationType,
        //                    PositionName = workerItem.PositionName,
        //                    UserName = workerItem.UserName,
        //                    Password = workerItem.Login
        //                };

        //                pollWorkerList.Add(newPollWorker);
        //            }

        //            // Add temp list to election
        //            _lists.PollWorkers = pollWorkerList;

        //            // Clear temp list
        //            pollWorkerList = null;
        //        }

        //    } // END USING

        //} // END LOAD POLLWORKERS

        //private void LoadProvisionalReasons()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var provisionalreasons = context.AvProvisionalReasons.ToList();

        //        if (provisionalreasons != null)
        //        {
        //            // Create temp list
        //            var provisionalReasonsList = new List<ProvisionalReasonModel>();

        //            // Copy fields to temp list
        //            foreach (var reason in provisionalreasons)
        //            {
        //                ProvisionalReasonModel newProvisionalReason = new ProvisionalReasonModel
        //                {
        //                    ProvisionalReasonId = reason.ProvisionalReasonId,
        //                    ProvisionalReasonDescription = reason.ProvisionalReason
        //                };

        //                provisionalReasonsList.Add(newProvisionalReason);
        //            }

        //            // Add temp list to election
        //            _lists.ProvisionalReasons = provisionalReasonsList;

        //            // Clear temp list
        //            provisionalReasonsList = null;
        //        }

        //    } // END USING

        //} // END LOAD PROVISIONALREASONS

        //private void LoadSpoiledReasons()
        //{
        //    // Create disposable database context
        //    using (var context = new ElectionContext())
        //    {
        //        // Get rejected reasons data from database
        //        var spoiledreasons = context.AvSpoiledReasons.ToList();

        //        if (spoiledreasons != null)
        //        {
        //            // Create temp list
        //            var spoiledReasonsList = new List<SpoiledReasonModel>();

        //            // Copy fields to temp list
        //            foreach (var reason in spoiledreasons)
        //            {
        //                SpoiledReasonModel newSpoiledReason = new SpoiledReasonModel
        //                {
        //                    SpoiledReasonId = reason.SpoiledReasonId,
        //                    SpoiledReasonDescription = reason.SpoiledReason
        //                };

        //                spoiledReasonsList.Add(newSpoiledReason);
        //            }

        //            // Add temp list to election
        //            _lists.SpoiledReasons = spoiledReasonsList;

        //            // Clear temp list
        //            spoiledReasonsList = null;
        //        }

        //    } // END USING

        //} // END LOAD SPOILEDREASONS

    } // END ELECTIONFACTORY

} // END NAMESPACE
