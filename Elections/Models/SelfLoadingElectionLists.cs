using VoterX.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterX.Core.Elections
{
    public class SelfLoadingElectionLists
    {
        private int? _electionId;
        public Exception Error;
        private string _connection;

        public SelfLoadingElectionLists(int ElectionId)
        {
            _electionId = ElectionId;
        }

        public SelfLoadingElectionLists(int ElectionId, string connection)
        {
            _electionId = ElectionId;
            _connection = connection;
        }

        private ElectionContext GetElectionContext()
        {
            if (_connection != null)
                return new ElectionContext(_connection);
            else
                return new ElectionContext();                
        }

        public void PreloadAllLists(int ElectionId)
        {
            Election = LoadElection(ElectionId);

            ApplicationRejectedReasons = LoadAppRejectedReasons();

            BallotRejectedReasons = LoadBallotRejectedReasons();

            BallotStyles = LoadBallotStyles(ElectionId);

            Jurisdictions = LoadJurisdictions();

            Locations = LoadLocations();

            LogCodes = LoadLogCodes();

            Partys = LoadParties();

            PollWorkers = LoadPollWorkers();

            Precincts = LoadPrecincts();

            ProvisionalReasons = LoadProvisionalReasons();

            SpoiledReasons = LoadSpoiledReasons();

            Tabulators = LoadTabulators();
        }

        #region Election
        private ElectionModel _election;
        public ElectionModel Election
        {
            get
            {
                if (_election == null && _electionId != null)
                {
                    // Load election data
                    _election = LoadElection(_electionId);
                }
                else if (_electionId == null)
                {
                    throw new Exception("Election Id not set.");
                }
                return _election;
            }
            set
            {
                _election = value;
            }
        }

        internal ElectionModel LoadElection(int? electionId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get election data from database
                    var electionData = context.SosElections.Where(elc => elc.ElectionId == electionId).FirstOrDefault();

                    if (electionData != null)
                    {
                        ElectionModel election = new ElectionModel();

                        // Load data into election
                        election.ElectionId = electionData.ElectionId;
                        election.CountyCode = electionData.CountyCode;
                        election.PollsOpenTime = electionData.PollsOpenTime;
                        election.PollsCloseTime = electionData.PollsCloseTime;
                        election.ElectionType = electionData.ElectionType;
                        election.CountyName = electionData.CountyName.ToUpper();
                        election.LastModified = electionData.LastModified;
                        election.ElectionName = electionData.ElectionName.Trim().ToUpper();
                        election.ElectionDate = electionData.ElectionDate;
                        election.AbsenteeBeginDate = electionData.AbsenteeBeginDate;
                        election.AbsenteeEndDate = electionData.AbsenteeEndDate;
                        election.UocavaBeginDate = electionData.UocavaBeginDate;
                        election.EarlyVotingBeginDate = electionData.EarlyVotingBeginDate;
                        election.EarlyVotingEndDate = electionData.EarlyVotingEndDate;
                        election.BookCloseDate = electionData.BookCloseDate;

                        return election;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    throw e;
                    return null;
                }

            } // END USING

        } // END LOADELECTIONDATA
        #endregion

        #region ApplicationRejectedReasons
        private List<ApplicationRejectedReasonModel> _applicationRejectedReasons;
        public List<ApplicationRejectedReasonModel> ApplicationRejectedReasons
        {
            get
            {
                if(_applicationRejectedReasons == null)
                {
                    // Load reasons from DB
                    _applicationRejectedReasons = LoadAppRejectedReasons();
                }
                return _applicationRejectedReasons;
            }
            set
            {
                _applicationRejectedReasons = value;
            }
        }

        private List<ApplicationRejectedReasonModel> LoadAppRejectedReasons()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var rejectedReasons = context.RejectedReasons.ToList();

                    if (rejectedReasons != null)
                    {
                        // Create temp list
                        var applicationRejectedReasonList = new List<ApplicationRejectedReasonModel>();

                        // Copy fields to temp list
                        foreach (var reason in rejectedReasons)
                        {
                            ApplicationRejectedReasonModel newRejectedReason = new ApplicationRejectedReasonModel
                            {
                                RejectedReasonId = reason.RejectedReasonId,
                                RejectedReasonDescription = reason.RejectedReason.ToUpper(),
                                ServiseCode = reason.ServisCode,
                                VoterXCode = reason.VoterXCode
                            };

                            applicationRejectedReasonList.Add(newRejectedReason);
                        }

                        // Add temp list to election
                        return applicationRejectedReasonList;

                        // Clear temp list
                        //applicationRejectedReasonList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING
        }
        #endregion

        #region BallotRejectedReasons
        private List<BallotRejectedReasonModel> _ballotRejectedReasons;
        public List<BallotRejectedReasonModel> BallotRejectedReasons
        {
            get
            {
                if (_ballotRejectedReasons == null)
                {
                    // Load reasons from DB
                    _ballotRejectedReasons = LoadBallotRejectedReasons();
                }
                return _ballotRejectedReasons;
            }
            set
            {
                _ballotRejectedReasons = value;
            }
        }

        private List<BallotRejectedReasonModel> LoadBallotRejectedReasons()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var rejectedReasons = context.BallotRejectedReasons.Where(brr => brr.Active == true).ToList();

                    if (rejectedReasons != null)
                    {
                        // Create temp list
                        var applicationRejectedReasonList = new List<BallotRejectedReasonModel>();

                        // Copy fields to temp list
                        foreach (var reason in rejectedReasons)
                        {
                            BallotRejectedReasonModel newRejectedReason = new BallotRejectedReasonModel
                            {
                                RejectedReasonId = reason.BallotRejectedReasonId,
                                RejectedReasonDescription = reason.RejectedReason.ToUpper(),
                                ServiseCode = reason.ServisCode,
                                NoticeStyle = reason.NoticeStyle,
                                Active = reason.Active,
                                LastModified = reason.LastModified
                            };

                            applicationRejectedReasonList.Add(newRejectedReason);
                        }

                        // Add temp list to election
                        return applicationRejectedReasonList;

                        // Clear temp list
                        //applicationRejectedReasonList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING
        }
        #endregion 

        #region BallotStyles
        private List<BallotStyleModel> _ballotStyles { get; set; }
        public List<BallotStyleModel> BallotStyles
        {
            get
            {
                if(_ballotStyles == null && _electionId != null)
                {
                    // Load Ballot Styles from DB
                    _ballotStyles = LoadBallotStyles(_electionId);
                }
                else if (_electionId == null)
                {
                    throw new Exception("Election Id not set.");
                }
                return _ballotStyles;
            }
            set
            {
                _ballotStyles = value;
            }
        }

        /// <summary>
        /// Query to return the entire dataset
        /// </summary>
        /// <returns></returns>
        private IQueryable<BallotStyleModel> BallotStyleQuery(ElectionContext context)
        {
            var ballotStyles = context.SosBallotStyles.Where(b => b.Active == true);

            var ballotStylePrecincts = context.SosBallotStylePrecincts.Where(p => p.Active == true);

            return ballotStyles
                .Join(
                ballotStylePrecincts,
                BallotStyles => BallotStyles.BallotStyleId,
                BSPrecinct => BSPrecinct.BallotStyleId,
                (BallotStyles, BSPrecinct) => new { BallotStyles, BSPrecinct }
                )                
                .Select(b => new BallotStyleModel
                {
                    BallotStyleId = b.BallotStyles.BallotStyleId,
                    ElectionId = b.BallotStyles.ElectionId,
                    CountyCode = b.BallotStyles.CountyCode,
                    PrecinctPartID = b.BSPrecinct.PrecinctPartId,
                    BallotStyleName = b.BallotStyles.BallotStyleName.ToUpper(),
                    BallotStyleFileName = b.BallotStyles.BallotStyleFileName,
                    Party = b.BallotStyles.Party
                });
        }

        // Uses LINQ Query to include the Precint Part ID (8/8/2019)
        private List<BallotStyleModel> LoadBallotStyles(int? ElectionId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    return BallotStyleQuery(context)
                        .Where(bs => bs.ElectionId == (int)ElectionId)
                        .ToList();
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }
            }
        }

        //private List<BallotStyleModel> LoadBallotStyles(int? ElectionId)
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
        //            return ballotStylesList;

        //            // Clear temp list
        //            //ballotStylesList = null;
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //    } // END USING

        //} // END LOAD BALLOTSTYLES
        #endregion

        #region Jurisdictions
        private List<JurisdictionModel> _jurisdictions;
        public List<JurisdictionModel> Jurisdictions
        {
            get
            {
                if(_jurisdictions == null)
                {
                    // Load Jurisdictions from DB
                    _jurisdictions = LoadJurisdictions();
                }
                return _jurisdictions;
            }
            set
            {
                _jurisdictions = value;
            }
        }

        private List<JurisdictionModel> LoadJurisdictions()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var jurisdictions = context.SosJurisdictions.Where(j => j.Active == true).ToList();

                    if (jurisdictions != null)
                    {
                        // Create temp list
                        var jurisdictionList = new List<JurisdictionModel>();

                        // Copy fields to temp list
                        foreach (var juryItem in jurisdictions)
                        {
                            JurisdictionModel newJurisdiction = new JurisdictionModel
                            {
                                JurisdictionId = juryItem.JurisdictionId,
                                JurisdictionType = juryItem.JurisdictionType,
                                JurisdictionName = juryItem.JurisdictionName.ToUpper(),
                                CountyCode = juryItem.CountyCode,
                                LastModified = juryItem.LastModified,
                                ModificationType = juryItem.ModificationType
                            };

                            jurisdictionList.Add(newJurisdiction);
                        }

                        // Add temp list to election
                        return jurisdictionList;

                        // Clear temp list
                        //jurisdictionList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD JURISDICTIONS

        public List<string> JurisdictionTypes
        {
            get
            {
                if (_jurisdictions == null)
                {
                    // Load Jurisdictions from DB
                    _jurisdictions = LoadJurisdictions();
                }
                return _jurisdictions.Select(j => j.JurisdictionType).Distinct().ToList();
            }
            //set
            //{
            //    _jurisdictions = value;
            //}
        }
        #endregion

        #region JurisdictionPrecincts
        //private List<JurisdictionPrecinctModel> _jurisdictionPrecincts;
        //public List<JurisdictionPrecinctModel> JurisdictionPrecincts
        //{
        //    get
        //    {
        //        if (_jurisdictionPrecincts == null)
        //        {
        //            // Load Jurisdictions from DB
        //            _jurisdictionPrecincts = LoadJurisdictionPrecincts();
        //        }
        //        return _jurisdictionPrecincts;
        //    }
        //    set
        //    {
        //        _jurisdictionPrecincts = value;
        //    }
        //}

        //private List<JurisdictionPrecinctModel> LoadJurisdictionPrecincts()
        //{
        //    // Create disposable database context
        //    using (var context = GetElectionContext())
        //    {
        //        try
        //        {
        //            // Get rejected reasons data from database
        //            var jurisdictionPrecincts = context.SosJurisdictionPrecincts.Where(jp => jp.Active == true).ToList();

        //            if (jurisdictionPrecincts != null)
        //            {
        //                // Create temp list
        //                var jurisdictionPrecinctList = new List<JurisdictionPrecinctModel>();

        //                // Copy fields to temp list
        //                foreach (var juryPItem in jurisdictionPrecincts)
        //                {
        //                    JurisdictionPrecinctModel newJurisdictionPrecinct = new JurisdictionPrecinctModel
        //                    {
        //                        JurisdictionId = juryPItem.JurisdictionId,
        //                        PrecinctPartId = juryPItem.PrecinctPartId,
        //                        CountyCode = juryPItem.CountyCode,
        //                        LastModified = juryPItem.LastModified,
        //                        Active = juryPItem.Active
        //                    };

        //                    jurisdictionPrecinctList.Add(newJurisdictionPrecinct);
        //                }

        //                // Add temp list to election
        //                return jurisdictionPrecinctList;

        //                // Clear temp list
        //                //jurisdictionList = null;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Error = e;
        //            //throw e;
        //            return null;
        //        }

        //    } // END USING

        //} // END LOAD JURISDICTIONS
        #endregion

        #region Locations
        private List<LocationModel> _locations { get; set; }
        public List<LocationModel> Locations
        {
            get
            {
                if(_locations == null)
                {
                    // Load Locations from DB
                    _locations = LoadLocations();
                }
                return _locations;
            }
            set
            {
                _locations = value;
            }
        }

        private List<LocationModel> LoadLocations()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var locations = context.SosLocations.Where(l => l.Active == true).ToList();

                    if (locations != null)
                    {
                        // Create temp list
                        var locationsList = new List<LocationModel>();

                        // Copy fields to temp list
                        foreach (var locationItem in locations)
                        {
                            LocationModel newLocation = new LocationModel
                            {
                                PollId = locationItem.PollId,
                                ElectionId = locationItem.ElectionId,
                                CountyCode = locationItem.CountyCode,
                                PlaceName = locationItem.PlaceName.ToUpper(),
                                Address = locationItem.Address,
                                City = locationItem.City,
                                Zip = locationItem.Zip,
                                FacilityContact = locationItem.FacilityContact,
                                FacilityPhone = locationItem.FacilityPhone,
                                LastModified = locationItem.LastModified,
                                ModificationType = locationItem.ModificationType
                            };

                            locationsList.Add(newLocation);
                        }

                        // Add temp list to election
                        return locationsList;

                        // Clear temp list
                        //locationsList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD LOCATIONS
        #endregion

        #region LogCodes
        private List<LogCodeModel> _logCodes { get; set; }
        public List<LogCodeModel> LogCodes
        {
            get
            {
                if(_logCodes == null)
                {
                    // Load Log Codes from DB
                    _logCodes = LoadLogCodes();
                }
                return _logCodes;
            }
            set
            {
                _logCodes = value;
            }
        }

        private List<LogCodeModel> LoadLogCodes()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var logcodes = context.AvLogCodes.ToList();

                    if (logcodes != null)
                    {
                        // Create temp list
                        var logCodesList = new List<LogCodeModel>();

                        // Copy fields to temp list
                        foreach (var logItem in logcodes)
                        {
                            LogCodeModel newLogCode = new LogCodeModel
                            {
                                LogCode = logItem.LogCode,
                                LogDescription = logItem.LogDescription.ToUpper(),
                                ServisCode = logItem.ServisCode,
                            };

                            logCodesList.Add(newLogCode);
                        }

                        // Add temp list to election
                        return logCodesList;

                        // Clear temp list
                        //logCodesList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }


            } // END USING

        } // END LOAD LOGCODES
        #endregion

        #region Partys
        private List<PartyModel> _partys { get; set; }
        public List<PartyModel> Partys
        {
            get
            {
                if(_partys == null)
                {
                    // Load Parties from DB
                    _partys = LoadParties();
                }
                return _partys;
            }
            set
            {
                _partys = value;
            }
        }

        private List<PartyModel> LoadParties()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var parties = context.SosVoters.Where(p => p.Party != null & p.Party != "").Select(p => p.Party).Distinct().ToList();

                    if (parties != null)
                    {
                        // Create temp list
                        var partyList = new List<PartyModel>();

                        // Copy fields to temp list
                        foreach (var partyItem in parties)
                        {
                            PartyModel newParty = new PartyModel
                            {
                                PartyCode = partyItem
                            };

                            partyList.Add(newParty);
                        }

                        // Add temp list to election
                        return partyList;

                        // Clear temp list
                        //partyList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD PARTIES
        #endregion

        #region PollWorkers
        private List<PollWorkerModel> _pollWorkers { get; set; }
        public List<PollWorkerModel> PollWorkers
        {
            get
            {
                if(_pollWorkers == null)
                {
                    // Load Poll Workers from DB
                    _pollWorkers = LoadPollWorkers();
                }
                return _pollWorkers;
            }
            set
            {
                _pollWorkers = value;
            }
        }

        private List<PollWorkerModel> LoadPollWorkers()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var pollworkers = context.SosPollWorkers.ToList();

                    if (pollworkers != null)
                    {
                        // Create temp list
                        var pollWorkerList = new List<PollWorkerModel>();

                        // Copy fields to temp list
                        foreach (var workerItem in pollworkers)
                        {
                            PollWorkerModel newPollWorker = new PollWorkerModel
                            {
                                VoterId = workerItem.VoterId,
                                ElectionId = workerItem.ElectionId,
                                CountyCode = workerItem.CountyCode,
                                FirstName = workerItem.FirstName,
                                MiddleName = workerItem.MiddleName,
                                LastName = workerItem.LastName,
                                Suffix = workerItem.Suffix,
                                Phone = workerItem.Phone,
                                Party = workerItem.Party,
                                PollId = workerItem.PollId,
                                Address = workerItem.Address,
                                City = workerItem.City,
                                State = workerItem.State,
                                Zip = workerItem.Zip,
                                LastModified = workerItem.LastModified,
                                ModificationType = workerItem.ModificationType,
                                PositionName = workerItem.PositionName,
                                UserName = workerItem.UserName,
                                Password = workerItem.Login
                            };

                            pollWorkerList.Add(newPollWorker);
                        }

                        // Add temp list to election
                        return pollWorkerList;

                        // Clear temp list
                        //pollWorkerList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD POLLWORKERS
        #endregion

        #region Precincts
        private List<PrecinctModel> _precincts { get; set; }
        public List<PrecinctModel> Precincts
        {
            get
            {
                if (_precincts == null)
                {
                    // Load Precincts from DB
                    _precincts = LoadPrecincts();
                }
                return _precincts;
            }
            set
            {
                _precincts = value;
            }
        }

        private List<PrecinctModel> LoadPrecincts()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var precincts = context.SosPrecincts.Where(p => p.Active == true).ToList();

                    if (precincts != null)
                    {
                        // Create temp list
                        var precinctList = new List<PrecinctModel>();

                        // Copy fields to temp list
                        foreach (var precinctItem in precincts)
                        {
                            PrecinctModel newPrecinct = new PrecinctModel
                            {
                                PrecinctPartID = precinctItem.PrecinctPartId,
                                PrecinctName = precinctItem.PrecinctName.ToUpper(),
                                PrecinctPart = precinctItem.PrecinctPart,
                                CountyCode = precinctItem.CountyCode,
                                LastModified = precinctItem.LastModified,
                                ModificationType = precinctItem.ModificationType,
                                Active = precinctItem.Active
                            };

                            precinctList.Add(newPrecinct);
                        }

                        return precinctList;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD PRECINCTS
        #endregion

        #region ProvisionalReasons
        private List<ProvisionalReasonModel> _provisionalReasons { get; set; }
        public List<ProvisionalReasonModel> ProvisionalReasons
        {
            get
            {
                if(_provisionalReasons == null)
                {
                    // Load Provisional Reasons from DB
                    _provisionalReasons = LoadProvisionalReasons();
                }
                return _provisionalReasons;
            }
            set
            {
                _provisionalReasons = value;
            }
        }

        private List<ProvisionalReasonModel> LoadProvisionalReasons()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var provisionalreasons = context.AvProvisionalReasons.ToList();

                    if (provisionalreasons != null)
                    {
                        // Create temp list
                        var provisionalReasonsList = new List<ProvisionalReasonModel>();

                        // Copy fields to temp list
                        foreach (var reason in provisionalreasons)
                        {
                            ProvisionalReasonModel newProvisionalReason = new ProvisionalReasonModel
                            {
                                ProvisionalReasonId = reason.ProvisionalReasonId,
                                ProvisionalReasonDescription = reason.ProvisionalReason.ToUpper()
                            };

                            provisionalReasonsList.Add(newProvisionalReason);
                        }

                        // Add temp list to election
                        return provisionalReasonsList;

                        // Clear temp list
                        //provisionalReasonsList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD PROVISIONALREASONS
        #endregion

        #region SpoiledReasons
        private List<SpoiledReasonModel> _spoiledReasons { get; set; }
        public List<SpoiledReasonModel> SpoiledReasons
        {
            get
            {
                if (_spoiledReasons == null)
                {
                    // Load Spoiled Reasons from DB
                    LoadSpoiledReasons();
                }
                return _spoiledReasons;
            }
            set
            {
                _spoiledReasons = value;
            }
        }

        private List<SpoiledReasonModel> LoadSpoiledReasons()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var spoiledreasons = context.AvSpoiledReasons.ToList();

                    if (spoiledreasons != null)
                    {
                        // Create temp list
                        var spoiledReasonsList = new List<SpoiledReasonModel>();

                        // Copy fields to temp list
                        foreach (var reason in spoiledreasons)
                        {
                            SpoiledReasonModel newSpoiledReason = new SpoiledReasonModel
                            {
                                SpoiledReasonId = reason.SpoiledReasonId,
                                SpoiledReasonDescription = reason.SpoiledReason.ToUpper()
                            };

                            spoiledReasonsList.Add(newSpoiledReason);
                        }

                        // Add temp list to election
                        return spoiledReasonsList;

                        // Clear temp list
                        //spoiledReasonsList = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD SPOILEDREASONS
        #endregion

        #region Tabulators
        private List<TabulatorModel> _tabulators { get; set; }
        public List<TabulatorModel> Tabulators
        {
            get
            {
                if (_tabulators == null)
                {
                    // Load Locations from DB
                    _tabulators = LoadTabulators();
                }
                return _tabulators;
            }
            set
            {
                _tabulators = value;
            }
        }

        private List<TabulatorModel> LoadTabulators()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                try
                {
                    // Get rejected reasons data from database
                    var locations = context.SosTabulators.ToList();

                    if (locations != null)
                    {
                        // Create temp list
                        var tabulatorsList = new List<TabulatorModel>();

                        // Copy fields to temp list
                        foreach (var tabulatorItem in locations)
                        {
                            TabulatorModel newTabulator = new TabulatorModel
                            {
                                TabulatorName = tabulatorItem.TabulatorName.ToUpper(),
                                PollId = tabulatorItem.PollId,
                                ElectionId = tabulatorItem.ElectionId,
                                CountyCode = tabulatorItem.CountyCode,
                                SerialNumber = tabulatorItem.SerialNumber.ToUpper(),
                                LastModified = tabulatorItem.LastModified,
                                ModificationType = tabulatorItem.ModificationType
                            };

                            tabulatorsList.Add(newTabulator);
                        }

                        // Add temp list to election
                        return tabulatorsList;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Error = e;
                    //throw e;
                    return null;
                }

            } // END USING

        } // END LOAD TABULATORS
        #endregion
    }
}
