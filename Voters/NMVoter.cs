using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.Context;
using VoterX.NMStateElection;
using VoterX.Core.Utilities;
using System.Data.Entity;
using VoterX.Core.Elections;
using VoterX.Core.Extensions;
using Newtonsoft.Json;
using VoterX.Core.Voters.Models;

namespace VoterX.Core.Voters
{
    public class NMVoter
    {
        public VoterDataModel Data { get; set; }
        public string ConnectionString { get; set; }
        public Exception Error { get; set; }
        private bool _ForceUpdate { get; set; }

        private ElectionContext GetElectionContext()
        {
            if(ConnectionString == null)
            {
                return new ElectionContext();
            }
            else
            {
                return new ElectionContext(ConnectionString);
            }
        }

        // Property to cleanly manage the Voter's ballot style
        public BallotStyleModel BallotStyle
        {
            get
            {
                if(Data != null && Data.BallotStyleID != null)
                {
                    return new BallotStyleModel
                    {
                        BallotStyleId = (int)Data.BallotStyleID,
                        BallotStyleName = Data.BallotStyle,
                        BallotStyleFileName = Data.BallotStyleFile
                    };
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (Data != null)
                {
                    var style = value;
                    Data.BallotStyleID = style.BallotStyleId;
                    Data.BallotStyle = style.BallotStyleName;
                    Data.BallotStyleFile = style.BallotStyleFileName;
                }
            }
        }

        public NMVoter()
        {
            Data = new VoterDataModel();
        }

        public NMVoter(VoterDataModel voterData)
        {
            Data = voterData;
            //ConnectionString = connectionString;
        }

        public override string ToString()
        {            
            return JsonConvert.SerializeObject(Data);
        }

        private void AddToUploadQueue(ElectionContext context, int voterId, int? pollId)
        {
            //// Add record to upload queue
            //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //newVoterSync.SyncId = Guid.NewGuid();
            //newVoterSync.VoterId = voterId;
            //newVoterSync.QueueTime = DateTime.Now;
            //newVoterSync.PollId = pollId.Value;
            //newVoterSync.Uploaded = false;

            //context.AvVotedRecordSyncs.Add(newVoterSync);
        }

        // Creates a copy of the Voted Record
        // ALWAYS add this method AFTER any changes have been made to the existing record or a new one was created
        private void CreateHistory(ElectionContext context, AvVotedRecord voter, string action)
        {
            CreateHistory(context, voter, action, false);
        }
        private void CreateHistory(ElectionContext context, AvVotedRecord voter, string action, bool init)
        {
            AvVotedRecordHistory newHistory = new AvVotedRecordHistory 
            {
                VotedRecordHistoryId = Guid.NewGuid(),
                HistoryDate = DateTime.Now,
                HistoryAction = action,
                HistoryInit = init,
                VoterId = voter.VoterId,
                LogCode = voter.LogCode,
                ElectionId = voter.ElectionId,
                PrecinctPartId = voter.PrecinctPartId,
                BallotStyleId = voter.BallotStyleId,
                PollId = voter.PollId,
                LastSynced = voter.LastSynced,
                CountyCode = voter.CountyCode,
                ApplicationIssued = voter.ApplicationIssued,
                ApplicationAccepted = voter.ApplicationAccepted,
                ApplicationRejected = voter.ApplicationRejected,
                DateIssued = voter.DateIssued,
                PrintedDate = voter.PrintedDate,
                AbsenteeId = voter.AbsenteeId,
                AbsenteeVoterType = voter.AbsenteeVoterType,
                ApplicationSource = voter.ApplicationSource,
                UocavaApplicationType = voter.UocavaApplicationType,
                UocavaVoterEmail = voter.UocavaVoterEmail,
                UocavaVoterFax = voter.UocavaVoterFax,
                BallotDeliveryMethod = voter.BallotDeliveryMethod,
                AddressType = voter.AddressType,
                AddressLine1 = voter.AddressLine1,
                AddressLine2 = voter.AddressLine2,
                City = voter.City,
                State = voter.State,
                Zip = voter.Zip,
                Country = voter.Country,
                TempAddress = voter.TempAddress,
                DateVoted = voter.DateVoted,
                Computer = voter.Computer,
                BallotNumber = voter.BallotNumber,
                SignRefused = voter.SignRefused,
                LocalOnly = true,
                UserId = voter.UserId,
                ActivityCode = voter.ActivityCode,
                ActivityDate = voter.ActivityDate,
                ProvisionalOnly = voter.ProvisionalOnly,
                //FledVoter = voter.FledVoter,
                //WrongVoter = voter.WrongVoter,
                NotTabulated = voter.NotTabulated,
                AppRejectedReason = voter.AppRejectedReason,
                SpoiledReason = voter.SpoiledReason,
                BcMailDate = voter.BcMailDate,
                ForcePropagate = voter.ForcePropagate,
                CreatedOnDate = voter.CreatedOnDate,
                IMBOut = voter.IMBOut,
                IMBIn = voter.IMBIn,
                VoterContactEmail = voter.VoterContactEmail,
                VoterContactPhone = voter.VoterContactPhone,
                BallotRejectedDate = voter.BallotRejectedDate,
                BallotRejectedReason = voter.BallotRejectedReason,
                ServisABSLastModified = voter.ServisABSLastModified,
                ServisVotedLastModified = voter.ServisVotedLastModified,
                IdRequired = voter.IdRequired
            };

            context.AvVotedRecordHistorys.Add(newHistory);

        }

        private void CreateHistory(ElectionContext context, AvActivity voter, string action)
        {
            CreateHistory(context, voter, action, false);
        }
        private void CreateHistory(ElectionContext context, AvActivity voter, string action, bool init)
        {
            AvVotedRecordHistory newHistory = new AvVotedRecordHistory
            {
                VotedRecordHistoryId = Guid.NewGuid(),
                HistoryDate = DateTime.Now,
                HistoryAction = action + " (" + Environment.MachineName.ToString() + "|" + Data.LocationID.ToString() + ")",
                HistoryInit = init,
                VoterId = voter.VoterId,
                LogCode = voter.LogCode,
                ElectionId = voter.ElectionId,
                PrecinctPartId = voter.PrecinctPartId,
                BallotStyleId = voter.BallotStyleId,
                PollId = voter.PollId,
                LastSynced = voter.LastSynced,
                CountyCode = voter.CountyCode,
                ApplicationIssued = voter.ApplicationIssued,
                ApplicationAccepted = voter.ApplicationAccepted,
                ApplicationRejected = voter.ApplicationRejected,
                DateIssued = voter.DateIssued,
                PrintedDate = voter.PrintedDate,
                AbsenteeId = voter.AbsenteeId,
                AbsenteeVoterType = voter.AbsenteeVoterType,
                ApplicationSource = voter.ApplicationSource,
                UocavaApplicationType = voter.UocavaApplicationType,
                UocavaVoterEmail = voter.UocavaVoterEmail,
                UocavaVoterFax = voter.UocavaVoterFax,
                BallotDeliveryMethod = voter.BallotDeliveryMethod,
                AddressType = voter.AddressType,
                AddressLine1 = voter.AddressLine1,
                AddressLine2 = voter.AddressLine2,
                City = voter.City,
                State = voter.State,
                Zip = voter.Zip,
                Country = voter.Country,
                TempAddress = voter.TempAddress,
                DateVoted = voter.DateVoted,
                Computer = voter.Computer,
                BallotNumber = voter.BallotNumber,
                SignRefused = voter.SignRefused,
                IdRequired = voter.IdRequired, // Always read from tblVoters
                LocalOnly = true,
                UserId = voter.UserId,
                ActivityCode = voter.ActivityCode,
                ActivityDate = voter.ActivityDate,
                ProvisionalOnly = voter.ProvisionalOnly,
                //FledVoter = voter.FledVoter,
                //WrongVoter = voter.WrongVoter,
                NotTabulated = voter.NotTabulated,
                AppRejectedReason = voter.AppRejectedReason,
                SpoiledReason = voter.SpoiledReason,
                BcMailDate = voter.BcMailDate,
                ForcePropagate = voter.IgnoreRules,
                CreatedOnDate = voter.CreatedOnDate,
                IMBOut = voter.IMBOut,
                IMBIn = voter.IMBIn,
                VoterContactEmail = voter.VoterContactEmail,
                VoterContactPhone = voter.VoterContactPhone,
                BallotRejectedDate = voter.BallotRejectedDate,
                BallotRejectedReason = voter.BallotRejectedReason,
                ServisABSLastModified = voter.ServisABSLastModified,
                ServisVotedLastModified = voter.ServisVotedLastModified,

                LastModified = voter.LastModified

            };

            context.AvVotedRecordHistorys.Add(newHistory);

        }

        //private void CreateVoidHistory(ElectionContext context, AvVotedRecord voter, string action)
        //{
        //    CreateVoidHistory(context, voter, action, false);
        //}
        private void CreateVoidHistory(ElectionContext context, AvVotedRecord voter, string action, int? pollId, int? computerId)
        {
            AvVotedRecordHistory newHistory = new AvVotedRecordHistory
            {
                VotedRecordHistoryId = Guid.NewGuid(),
                HistoryDate = DateTime.Now,
                HistoryAction = action + " (" + Environment.MachineName.ToString() + "|" + Data.LocationID.ToString() + ")",
                HistoryInit = false,
                VoterId = voter.VoterId,
                LogCode = 2,
                ElectionId = voter.ElectionId,
                PrecinctPartId = voter.PrecinctPartId,
                BallotStyleId = voter.BallotStyleId,
                PollId = pollId,
                LastSynced = voter.LastSynced,
                CountyCode = voter.CountyCode,
                ApplicationIssued = voter.ApplicationIssued,
                ApplicationAccepted = voter.ApplicationAccepted,
                ApplicationRejected = voter.ApplicationRejected,
                DateIssued = voter.DateIssued,
                PrintedDate = voter.PrintedDate,
                AbsenteeId = voter.AbsenteeId,
                AbsenteeVoterType = voter.AbsenteeVoterType,
                ApplicationSource = voter.ApplicationSource,
                UocavaApplicationType = voter.UocavaApplicationType,
                UocavaVoterEmail = voter.UocavaVoterEmail,
                UocavaVoterFax = voter.UocavaVoterFax,
                BallotDeliveryMethod = voter.BallotDeliveryMethod,
                AddressType = voter.AddressType,
                AddressLine1 = voter.AddressLine1,
                AddressLine2 = voter.AddressLine2,
                City = voter.City,
                State = voter.State,
                Zip = voter.Zip,
                Country = voter.Country,
                TempAddress = voter.TempAddress,
                DateVoted = voter.DateVoted,
                Computer = computerId,
                BallotNumber = voter.BallotNumber,
                SignRefused = voter.SignRefused,
                LocalOnly = true,
                UserId = voter.UserId,
                ActivityCode = voter.ActivityCode,
                ActivityDate = DateTime.Now,
                ProvisionalOnly = voter.ProvisionalOnly,
                //FledVoter = voter.FledVoter,
                //WrongVoter = voter.WrongVoter,
                NotTabulated = voter.NotTabulated,
                SpoiledReason = voter.SpoiledReason,
                BcMailDate = voter.BcMailDate,
                ForcePropagate = voter.ForcePropagate,
                CreatedOnDate = voter.CreatedOnDate,
                IMBOut = voter.IMBOut,
                IMBIn = voter.IMBIn,
                VoterContactEmail = voter.VoterContactEmail,
                VoterContactPhone = voter.VoterContactPhone,
                BallotRejectedDate = voter.BallotRejectedDate,
                BallotRejectedReason = voter.BallotRejectedReason,
                ServisABSLastModified = voter.ServisABSLastModified,
                ServisVotedLastModified = voter.ServisVotedLastModified,
                IdRequired = voter.IdRequired
            };

            context.AvVotedRecordHistorys.Add(newHistory);

        }

        public void Localize(int election, int poll, int? computer, int? user)
        {
            Data.ElectionID = election;
            Data.PollID = poll;
            Data.ComputerID = computer;
            Data.UserId = user;

            Data.LocationID = poll;
        }
        public void Localize(int election, int user, string userName, int poll, string pollName, int computer)
        {
            Data.ElectionID = election;
            Data.UserId = user;
            Data.UserName = userName;
            Data.PollID = poll;
            Data.PollName = pollName;
            Data.ComputerID = computer;

            Data.LocationID = poll;

        }

        public int? ValidatePrecinctPoll(int poll)
        {
            // Create disposable database context            
            using (var context = GetElectionContext())
            {
                try
                {
                    return context.SosLocationPrecincts.Where(lp => lp.PrecinctPartId == Data.PrecinctPartID && lp.PollId == poll).FirstOrDefault().PollId;
                }
                catch
                {
                    // When precinct location is not found return a null value
                    return null;
                }
            }
        }

        private Election GetElection()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                return context.SosElections.Find(Data.ElectionID);
            }
        }

        public bool GenerateIntelligentBarcodes(string barcode, string serviceTypeOut, string serviceTypeIn, string prefix, int computer, string returnZip)
        {
            IntelligentBarcodeModel imb = new IntelligentBarcodeModel()
            {
                Barcode = barcode,
                OutServiceType = serviceTypeOut,
                InServiceType = serviceTypeIn,
                Prefix = prefix,
                Computer = computer,
                ReturnZip = returnZip
            };

            return GenerateIntelligentBarcodes(imb);
        }
        public bool GenerateIntelligentBarcodes(IntelligentBarcodeModel imbParam)
        {
            bool result = false;

            using (var context = GetElectionContext())
            {
                string barId = imbParam.Barcode;
                string serviceTypeOut = imbParam.OutServiceType;
                string serviceTypeIn = imbParam.InServiceType;
                //string outMailerId = "123456";
                //string inMailerId = "654321";
                //string mailerId = imbParam.MailerId;
                string outMailerId = imbParam.OutMailerId;
                string inMailerId = imbParam.InMailerId;

                // Get max Serial Number
                //int? sId = context.AvIntelligentMailingBarcodes.Max(imb => imb.SerialNumber);
                int? sId = context.AvIntelligentMailingBarcodes.Where(imb => imb.SerialPrefix == imbParam.Prefix && imb.Computer == imbParam.Computer).Max(imb => imb.SerialNumber);

                // Increment Serial Number
                if (sId == null) sId = 0;
                sId++;

                // Convert serial int to 9 character string
                string serial = "000000" + sId.ToString();
                //string serialNo = imbParam.Prefix + imbParam.Computer.ToString() + serial.Substring(serial.Length - 6, 6);
                string outSerialNo = imbParam.Computer.ToString() + serial.Substring(serial.Length - 5, 5);
                
                // Inverse Serial Number
                int? invSid = 50000 + sId;
                string inSerialNo = imbParam.Computer.ToString() + invSid.ToString();

                // Pad Zip Codes (DO NOT PAD ZIPCODES! IMB Istructions kit600.pdf page 8)
                //string outZip = "000000000";
                //string inZip = "000000000";
                //outZip = (Data.DeliveryZip.Replace("-", "") + "000000000").Substring(0, 9);
                //inZip = (imbParam.ReturnZip.Replace("-", "") + "000000000").Substring(0, 9);

                string outZip = "";
                string inZip = "";
                
                if (Data.DeliveryZip != null)
                {
                    // Remove dashes and all special characters
                    //outZip = Data.DeliveryZip.Replace("-", "");
                    outZip = outZip.RemoveNoneNumeric();

                    // Check length and isNumeric
                    if ((outZip.Length == 5 || outZip.Length == 9) && Int32.TryParse(outZip, out int n))
                    {

                    }
                    else
                    {
                        //throw new Exception("Voter's zip code is invalid");
                        outZip = "";
                    }
                }
                else
                {
                    //throw new Exception("Voter's zip code cannot be null");
                    outZip = "";
                }

                if (imbParam.ReturnZip != null)
                {
                    // Remove dashes
                    inZip = imbParam.ReturnZip.Replace("-", "");
                    // Check length and isNumeric
                    if ((inZip.Length == 5 || inZip.Length == 9) && Int32.TryParse(inZip, out int n))
                    {

                    }
                    else
                    {
                        //throw new Exception("Return zip code is invalid");
                        inZip = "";
                    }
                }
                else
                {
                    //throw new Exception("Return zip code cannot be null");
                    inZip = "";
                } 

                // Concatenate numbers into 31 character IMB
                string OutGoingIMB = (barId + serviceTypeOut + outMailerId + outSerialNo + outZip).RemoveNoneNumeric();
                string InComingIMB = (barId + serviceTypeIn + inMailerId + inSerialNo + inZip).RemoveNoneNumeric();

                if(OutGoingIMB.Length > 31 || InComingIMB.Length > 31)
                {
                    //return false;
                    throw new Exception("Error Generating IMB: Length exceeds USPS limits");
                }

                if (Int32.TryParse(Data.VoterID, out int intVoterId))
                {
                    // Create new IMB record
                    AvIMB newSerialNumber = new AvIMB()
                    {
                        IMBId = Guid.NewGuid(),
                        VoterId = intVoterId,
                        DateAssigned = DateTime.Now,
                        SerialPrefix = imbParam.Prefix,
                        Computer = imbParam.Computer,
                        SerialNumber = sId,
                        IMBOut = OutGoingIMB,
                        IMBIn = InComingIMB,
                        LastModified = DateTime.Now,
                        LocalOnly = true
                    };

                    try
                    {
                        // Insert new record
                        context.AvIntelligentMailingBarcodes.Add(newSerialNumber);
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return false;
                }

                // Update voter object
                Data.OutGoingIMB = OutGoingIMB;
                Data.InComingIMB = InComingIMB;

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Set the temp address fields if temp flag is set
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateTempAddress()
        {
            if(Data.TempAddress == true)
            {
                Data.TempAddress1 = Data.DeliveryAddress1;
                Data.TempAddress2 = Data.DeliveryAddress2;
                Data.TempCity = Data.DeliveryCity;
                Data.TempState = Data.State;
                Data.TempZip = Data.DeliveryZip;
                Data.TempCountry = Data.Country;
            }
        }

        private void UpdateVoted(string action)
        {
            UpdateVoted(action, false);
        }
        private void UpdateVoted(string action, bool changeCreatedDate)
        {
            // Create disposable database context            
            using (var context = GetElectionContext())
            {
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord == null)
                {
                    // INSERT NEW VOTED RECORD

                    // Check for NULL Created On Date
                    DateTime createdOnDate = Data.CreatedOnDate ?? DateTime.MinValue;
                    if (createdOnDate == DateTime.MinValue && Data.LogCode > 5)
                    {
                        // Set created on date the first time a ballot is voted
                        createdOnDate = DateTime.Now;
                    }

                    // Create local record template
                    AvActivity newVotedRecord = new AvActivity
                    {
                        // Map fields to template
                        VoterId = Int32.Parse(Data.VoterID),
                        LogCode = Data.LogCode,
                        ElectionId = Data.ElectionID,
                        PrecinctPartId = Data.PrecinctPartID,
                        BallotStyleId = Data.BallotStyleID,
                        PollId = Data.PollID,
                        LastSynced = DateTime.Parse("1/1/1"),
                        CountyCode = Data.County,

                        ApplicationIssued = Data.ApplicationIssued,
                        ApplicationAccepted = Data.ApplicationAccepted,
                        ApplicationRejected = Data.ApplicationRejected,
                        AppRejectedReason = Data.ApplicationRejectedReason,
                        DateIssued = Data.BallotIssued,
                        PrintedDate = Data.BallotPrinted,
                        AbsenteeId = Data.AbsenteeId,
                        AbsenteeVoterType = Data.AbsenteeType,
                        ApplicationSource = Data.ApplicationSource,
                        UocavaApplicationType = Data.UocavaApplicationType,
                        UocavaVoterEmail = Data.UocavaVoterEmail,
                        UocavaVoterFax = Data.UocavaVoterFax,
                        BallotDeliveryMethod = Data.BallotDeliveryMethod,

                        // Add voters address
                        AddressType = Data.AddressType,
                        AddressLine1 = Data.DeliveryAddress1,
                        AddressLine2 = Data.DeliveryAddress2,
                        City = Data.DeliveryCity,
                        State = Data.DeliveryState,
                        Zip = Data.DeliveryZip,
                        Country = Data.DeliveryCountry,
                        TempAddress = (bool)Data.TempAddress.Value,

                        DateVoted = Data.VotedDate,
                        Computer = Data.ComputerID,
                        BallotNumber = Data.BallotNumber,
                        SignRefused = (bool)(Data.SignRefused??false),
                        IdRequired = Data.IDRequired,
                        UserId = Data.UserId,                        
                        ActivityCode = Data.ActivityCode,
                        ActivityDate = DateTime.Now,
                        ProvisionalOnly = Data.ProvisionalOnly,
                        NotTabulated = Data.NotTabulated,
                        //AppRejectedReason = Data.ApplicationRejectedReason,
                        SpoiledReason = Data.SpoiledReason,
                        BcMailDate = Data.BcMailDate,

                        // Intelligent Barcodes
                        IMBOut = Data.OutGoingIMB,
                        IMBIn = Data.InComingIMB,

                        VoterContactPhone = Data.Phone,
                        VoterContactEmail = Data.Email,

                        BallotRejectedDate = Data.BallotRejectedDate,
                        BallotRejectedReason = Data.BallotRejectedReason,

                        ServisABSLastModified = Data.ServisABSLastModified,
                        ServisVotedLastModified = Data.ServisVotedLastModified,

                        BatchId = Data.BatchID,

                        // Only qualify this field the first time a record is created
                        CreatedOnDate = createdOnDate,

                        IgnoreRules = _ForceUpdate,
                        LastModified = DateTime.Now,
                        LocalOnly = true
                    };

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, newVotedRecord, action, true);

                    try
                    {
                        // Insert new voted record into database
                        context.AvActivities.Add(newVotedRecord);
                        //context.Entry(newVotedRecord).State = EntityState.Added;
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    // UPDATE EXISTING RECORD

                    // Update voted record
                    votedRecord.DateVoted = Data.VotedDate;
                    votedRecord.PrintedDate = Data.BallotPrinted;
                    votedRecord.PollId = Data.PollID;
                    votedRecord.Computer = Data.ComputerID;
                    votedRecord.BallotStyleId = Data.BallotStyleID;
                    votedRecord.PrecinctPartId = Data.PrecinctPartID;
                    votedRecord.SignRefused = Data.SignRefused??false;
                    votedRecord.UserId = Data.UserId;
                    votedRecord.LogCode = Data.LogCode;
                    votedRecord.ActivityCode = Data.ActivityCode;
                    votedRecord.ActivityDate = DateTime.Now;
                    votedRecord.NotTabulated = Data.NotTabulated;
                    votedRecord.LastSynced = DateTime.Parse("1/1/1");

                    // Add voters address
                    votedRecord.AddressLine1 = Data.DeliveryAddress1;
                    votedRecord.AddressLine2 = Data.DeliveryAddress2;
                    votedRecord.City = Data.DeliveryCity;
                    votedRecord.State = Data.DeliveryState;
                    votedRecord.Zip = Data.DeliveryZip;
                    votedRecord.Country = Data.DeliveryCountry;
                    votedRecord.AddressType = Data.AddressType;

                    votedRecord.BallotNumber = Data.BallotNumber;

                    // Absentee Ballots and Applications
                    votedRecord.ApplicationAccepted = Data.ApplicationAccepted;
                    votedRecord.ApplicationSource = Data.ApplicationSource;
                    votedRecord.ApplicationRejected = Data.ApplicationRejected;
                    votedRecord.AppRejectedReason = Data.ApplicationRejectedReason;
                    votedRecord.BallotDeliveryMethod = Data.BallotDeliveryMethod;
                    votedRecord.ApplicationIssued = Data.ApplicationIssued;
                    votedRecord.AbsenteeVoterType = Data.AbsenteeType;
                    votedRecord.DateIssued = Data.BallotIssued;

                    votedRecord.BallotRejectedDate = Data.BallotRejectedDate;
                    votedRecord.BallotRejectedReason = Data.BallotRejectedReason;

                    // Intelligent Barcodes
                    votedRecord.IMBOut = Data.OutGoingIMB;
                    votedRecord.IMBIn = Data.InComingIMB;

                    votedRecord.VoterContactPhone = Data.Phone;
                    votedRecord.VoterContactEmail = Data.Email;

                    // Update Created On Date when NULL, only on issued ballots
                    if (votedRecord.CreatedOnDate == DateTime.MinValue && Data.LogCode > 5)
                    {
                        votedRecord.CreatedOnDate = DateTime.Now;
                    }
                    else if (changeCreatedDate == true)
                    {
                        votedRecord.CreatedOnDate = Data.CreatedOnDate.Value;
                    }

                    votedRecord.LastModified = DateTime.Now;
                    votedRecord.IgnoreRules = _ForceUpdate;
                    votedRecord.LocalOnly = true;

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, action, false);

                    try
                    {
                        // Update the voted record in the database
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                }// End null record check

            }// End using context

        } // End Update Voted

        public AvActivity ConvertToActivity()
        {
            return new AvActivity()
            {
                // Map fields to template
                VoterId = Int32.Parse(Data.VoterID),
                LogCode = Data.LogCode,
                ElectionId = Data.ElectionID,
                PrecinctPartId = Data.PrecinctPartID,
                BallotStyleId = Data.BallotStyleID,
                PollId = Data.PollID,
                LastSynced = DateTime.Parse("1/1/1"),
                CountyCode = Data.County,

                ApplicationIssued = Data.ApplicationIssued,
                ApplicationAccepted = Data.ApplicationAccepted,
                ApplicationRejected = Data.ApplicationRejected,
                DateIssued = Data.BallotIssued,
                PrintedDate = Data.BallotPrinted,
                AbsenteeId = Data.AbsenteeId,
                AbsenteeVoterType = Data.AbsenteeType,
                ApplicationSource = Data.ApplicationSource,
                UocavaApplicationType = Data.UocavaApplicationType,
                UocavaVoterEmail = Data.UocavaVoterEmail,
                UocavaVoterFax = Data.UocavaVoterFax,
                BallotDeliveryMethod = Data.BallotDeliveryMethod,

                // Add voters address
                AddressType = Data.AddressType,
                AddressLine1 = Data.DeliveryAddress1,
                AddressLine2 = Data.DeliveryAddress2,
                City = Data.DeliveryCity,
                State = Data.DeliveryState,
                Zip = Data.DeliveryZip,
                Country = Data.DeliveryCountry,
                TempAddress = (bool)Data.TempAddress.Value,

                DateVoted = Data.VotedDate,
                Computer = Data.ComputerID,
                BallotNumber = Data.BallotNumber,
                SignRefused = (bool)Data.SignRefused,
                IdRequired = Data.IDRequired,
                UserId = Data.UserId,
                ActivityCode = Data.ActivityCode,
                ActivityDate = DateTime.Now,
                ProvisionalOnly = Data.ProvisionalOnly,
                NotTabulated = Data.NotTabulated,
                AppRejectedReason = Data.ApplicationRejectedReason,
                SpoiledReason = Data.SpoiledReason,
                BcMailDate = Data.BcMailDate,

                // Intelligent Barcodes
                IMBOut = Data.OutGoingIMB,
                IMBIn = Data.InComingIMB,

                VoterContactPhone = Data.Phone,
                VoterContactEmail = Data.Email,

                BallotRejectedDate = Data.BallotRejectedDate,
                BallotRejectedReason = Data.BallotRejectedReason,

                ServisABSLastModified = Data.ServisABSLastModified,
                ServisVotedLastModified = Data.ServisVotedLastModified,

                BatchId = Data.BatchID,

                // Only qualify this field the first time a record is created
                CreatedOnDate = Data.CreatedOnDate??DateTime.MinValue,

                IgnoreRules = _ForceUpdate,
                LastModified = DateTime.Now,
                LocalOnly = true
            };
        }

        #region VCCMethods

        /// <summary>
        /// Mark Voter as Voted at Polls
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void VotedAtPolls(int ElectionMode)
        {
            VotedAtPolls(ElectionMode, false);
        }
        public void VotedAtPolls(int ElectionMode, bool Voided)
        {
            if ((Data.LogCode < 5 || Data.LogCode == null) || Voided == true)
            {
                Data.VotedDate = DateTime.Now;
                Data.BallotPrinted = DateTime.Now;
                Data.LogCode = ElectionMode == 2 ? LogCodeConstants.VotedAtPolls : LogCodeConstants.EarlyVoting;
                Data.ActivityCode = ElectionMode == 2 ? "P" : "E";
                Data.ActivityDate = DateTime.Now;
                Data.NotTabulated = false;

                // Add voters address
                Data.DeliveryAddress1 = Data.Address1;
                Data.DeliveryAddress2 = Data.Address2;
                Data.DeliveryCity = Data.City;
                Data.DeliveryState = Data.State;
                Data.DeliveryZip = Data.Zip;
                Data.DeliveryCountry = Data.Country;

                if (Data.CreatedOnDate == DateTime.MinValue)
                {
                    Data.CreatedOnDate = DateTime.Now;
                }

                UpdateVoted("Voted at polls");
            }

            //// Create disposable database context            
            //using (var context = new ElectionContext())
            //{
            //    //var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord == null)
            //    {
            //        // INSERT NEW VOTED RECORD

            //        // Create voted record template
            //        AvActivity newVotedRecord = new AvActivity
            //        {
            //            // Map fields to template
            //            VoterId = Int32.Parse(Data.VoterID),
            //            CountyCode = Data.County,
            //            ElectionId = Data.ElectionID,
            //            DateVoted = DateTime.Now,
            //            PrintedDate = DateTime.Now,
            //            PollId = Data.PollID,
            //            Computer = Data.ComputerID,
            //            BallotStyleId = Data.BallotStyleID,
            //            PrecinctPartId = Data.PrecinctPartID,
            //            SignRefused = (bool)Data.SignRefused.Value,
            //            UserId = Data.UserId,
            //            LogCode = ElectionMode == 2 ? LogCodeConstants.VotedAtPolls : LogCodeConstants.EarlyVoting,
            //            ActivityCode = ElectionMode == 2 ? "P" : "E",
            //            ActivityDate = DateTime.Now,
            //            //FledVoter = false,
            //            //WrongVoter = false,
            //            NotTabulated = false,
            //            TempAddress = (bool)Data.TempAddress.Value,
            //            //LocalOnly = true,
            //            LastSynced = DateTime.Parse("1/1/1"),

            //            // Add voters address
            //            AddressLine1 = Data.Address1,
            //            AddressLine2 = Data.Address2,
            //            City = Data.City,
            //            State = Data.State,
            //            Zip = Data.Zip,
            //            Country = Data.Country,

            //            BallotNumber = Data.BallotNumber,

            //            // Only qualify this field the first time a record is created
            //            CreatedOnDate = DateTime.Now
            //        };

            //        //// Add record to upload queue
            //        //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //        //newVoterSync.SyncId = Guid.NewGuid();
            //        //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //        //newVoterSync.QueueTime = DateTime.Now;

            //        //context.AvVotedRecordSyncs.Add(newVoterSync);

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, newVotedRecord, "Voted at polls", true);

            //        try
            //        {
            //            // Insert new voted record into database
            //            context.AvVotedRecords.Add(newVotedRecord);
            //            //context.Entry(newVotedRecord).State = EntityState.Added;
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        // Check voter status
            //        if (votedRecord.LogCode < 5)
            //        {
            //            // UPDATE EXISTING RECORD

            //            // Update voted record
            //            votedRecord.DateVoted = DateTime.Now;
            //            votedRecord.PrintedDate = DateTime.Now;
            //            votedRecord.PollId = Data.PollID;
            //            votedRecord.Computer = Data.ComputerID;
            //            votedRecord.BallotStyleId = Data.BallotStyleID;
            //            votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //            votedRecord.SignRefused = Data.SignRefused.Value;
            //            votedRecord.UserId = Data.UserId;
            //            votedRecord.LogCode = ElectionMode == 2 ? LogCodeConstants.VotedAtPolls : LogCodeConstants.EarlyVoting;
            //            votedRecord.ActivityCode = ElectionMode == 2 ? "P" : "E";
            //            votedRecord.ActivityDate = DateTime.Now;
            //            //votedRecord.FledVoter = false;
            //            //votedRecord.WrongVoter = false;
            //            votedRecord.NotTabulated = false;
            //            //votedRecord.LocalOnly = true;
            //            votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //            votedRecord.BallotNumber = Data.BallotNumber;

            //            votedRecord.BallotRejectedDate = null;
            //            votedRecord.BallotRejectedReason = null;

            //            //// Add record to upload queue
            //            //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //            //newVoterSync.SyncId = Guid.NewGuid();
            //            //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //            //newVoterSync.QueueTime = DateTime.Now;

            //            //context.AvVotedRecordSyncs.Add(newVoterSync);

            //            // Update Created On Date when NULL
            //            if (votedRecord.CreatedOnDate == DateTime.MinValue)
            //            {
            //                votedRecord.CreatedOnDate = DateTime.Now;
            //            }

            //            // PREPARE UPLOAD
            //            AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //            // CREATE VOTED HISTORY LOG
            //            CreateHistory(context, votedRecord, "Voted at polls");

            //            try
            //            {
            //                // Update the voted record in the database
            //                context.SaveChanges();
            //            }
            //            catch (Exception e)
            //            {
            //                throw e;
            //            }
            //        }
            //        else
            //        {
            //            throw new Exception("This voter has already voted with code: " + votedRecord.LogCode.ToString());
            //        }

            //    } // End null record check

            //} // End using context

        } // End Voted at Polls

        //public void VotedAtPolls(int ElectionMode, bool Voided)
        //{
        //    // Create disposable database context            
        //    using (var context = GetElectionContext())
        //    {
        //        var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
        //        if (votedRecord == null)
        //        {
        //            // INSERT NEW VOTED RECORD

        //            // Create voted record template
        //            AvVotedRecord newVotedRecord = new AvVotedRecord
        //            {

        //                // Map fields to template
        //                VoterId = Int32.Parse(Data.VoterID),
        //                CountyCode = Data.County,
        //                ElectionId = Data.ElectionID,
        //                DateVoted = DateTime.Now,
        //                PrintedDate = DateTime.Now,
        //                PollId = Data.PollID,
        //                Computer = Data.ComputerID,
        //                BallotStyleId = Data.BallotStyleID,
        //                PrecinctPartId = Data.PrecinctPartID,
        //                SignRefused = Data.SignRefused.Value,
        //                UserId = Data.UserId,
        //                LogCode = ElectionMode == 2 ? LogCodeConstants.VotedAtPolls : LogCodeConstants.EarlyVoting,
        //                ActivityCode = ElectionMode == 2 ? "P" : "E",
        //                ActivityDate = DateTime.Now,
        //                //FledVoter = false,
        //                //WrongVoter = false,
        //                NotTabulated = false,
        //                TempAddress = (bool)Data.TempAddress.Value,
        //                //LocalOnly = true,
        //                LastSynced = DateTime.Parse("1/1/1"),

        //                // Add voters address
        //                AddressLine1 = Data.Address1,
        //                AddressLine2 = Data.Address2,
        //                City = Data.City,
        //                State = Data.State,
        //                Zip = Data.Zip,
        //                Country = Data.Country,

        //                BallotNumber = Data.BallotNumber,

        //                // Only qualify this field the first time a record is created
        //                CreatedOnDate = DateTime.Now
        //            };

        //            //// Add record to upload queue
        //            //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
        //            //newVoterSync.SyncId = Guid.NewGuid();
        //            //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
        //            //newVoterSync.QueueTime = DateTime.Now;

        //            //context.AvVotedRecordSyncs.Add(newVoterSync);

        //            // PREPARE UPLOAD
        //            AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

        //            // CREATE VOTED HISTORY LOG
        //            CreateHistory(context, newVotedRecord, "Voted at polls", true);

        //            try
        //            {
        //                // Insert new voted record into database
        //                context.AvVotedRecords.Add(newVotedRecord);
        //                //context.Entry(newVotedRecord).State = EntityState.Added;
        //                context.SaveChanges();
        //            }
        //            catch (Exception e)
        //            {
        //                throw e;
        //            }
        //        }
        //        else
        //        {
        //            // Check voter status
        //            if (votedRecord.LogCode < 5 || Voided)
        //            {
        //                // UPDATE EXISTING RECORD

        //                // Update voted record
        //                votedRecord.DateVoted = DateTime.Now;
        //                votedRecord.PrintedDate = DateTime.Now;
        //                votedRecord.PollId = Data.PollID;
        //                votedRecord.Computer = Data.ComputerID;
        //                votedRecord.BallotStyleId = Data.BallotStyleID;
        //                votedRecord.PrecinctPartId = Data.PrecinctPartID;
        //                votedRecord.SignRefused = Data.SignRefused.Value;
        //                votedRecord.UserId = Data.UserId;
        //                votedRecord.LogCode = ElectionMode == 2 ? LogCodeConstants.VotedAtPolls : LogCodeConstants.EarlyVoting;
        //                votedRecord.ActivityCode = ElectionMode == 2 ? "P" : "E";
        //                votedRecord.ActivityDate = DateTime.Now;
        //                //votedRecord.FledVoter = false;
        //                //votedRecord.WrongVoter = false;
        //                votedRecord.NotTabulated = false;
        //                //votedRecord.LocalOnly = true;
        //                votedRecord.LastSynced = DateTime.Parse("1/1/1");

        //                votedRecord.BallotNumber = Data.BallotNumber;

        //                votedRecord.BallotRejectedDate = null;
        //                votedRecord.BallotRejectedReason = null;

        //                //// Add record to upload queue
        //                //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
        //                //newVoterSync.SyncId = Guid.NewGuid();
        //                //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
        //                //newVoterSync.QueueTime = DateTime.Now;

        //                //context.AvVotedRecordSyncs.Add(newVoterSync);

        //                // Update Created On Date when NULL
        //                if (votedRecord.CreatedOnDate == DateTime.MinValue)
        //                {
        //                    votedRecord.CreatedOnDate = DateTime.Now;
        //                }

        //                // PREPARE UPLOAD
        //                AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

        //                // CREATE VOTED HISTORY LOG
        //                CreateHistory(context, votedRecord, "Voted at polls");

        //                try
        //                {
        //                    // Update the voted record in the database
        //                    context.SaveChanges();
        //                }
        //                catch (Exception e)
        //                {
        //                    throw e;
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("This voter has already voted with code: " + votedRecord.LogCode.ToString());
        //            }

        //        } // End null record check

        //    } // End using context

        //} // End Voted at Polls

        public void VotedAtPolls(LogCodes logCode)
        {
            VotedAtPolls((int)logCode, false);

            //if (Data.LogCode < 5 || Data.LogCode == null)
            //{
            //    Data.VotedDate = DateTime.Now;
            //    Data.BallotPrinted = DateTime.Now;
            //    Data.LogCode = (int)logCode;
            //    Data.ActivityCode = LogCodeConstants.ConvertToActivity(logCode);
            //    Data.ActivityDate = DateTime.Now;
            //    Data.NotTabulated = false;

            //    // Add voters address
            //    Data.DeliveryAddress1 = Data.Address1;
            //    Data.DeliveryAddress2 = Data.Address2;
            //    Data.DeliveryCity = Data.City;
            //    Data.DeliveryState = Data.State;
            //    Data.DeliveryZip = Data.Zip;
            //    Data.DeliveryCountry = Data.Country;

            //    UpdateVoted("Voted at polls");
            //}

            //// Create disposable database context            
            //using (var context = GetElectionContext())
            //{
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord == null)
            //    {
            //        // INSERT NEW VOTED RECORD

            //        // Create voted record template
            //        AvVotedRecord newVotedRecord = new AvVotedRecord
            //        {

            //            // Map fields to template
            //            VoterId = Int32.Parse(Data.VoterID),
            //            CountyCode = Data.County,
            //            ElectionId = Data.ElectionID,
            //            DateVoted = DateTime.Now,
            //            PrintedDate = DateTime.Now,
            //            PollId = Data.PollID,
            //            Computer = Data.ComputerID,
            //            BallotStyleId = Data.BallotStyleID,
            //            PrecinctPartId = Data.PrecinctPartID,
            //            SignRefused = Data.SignRefused.Value,
            //            UserId = Data.UserId,
            //            LogCode = (int)logCode,
            //            ActivityCode = LogCodeConstants.ConvertToActivity(logCode),
            //            ActivityDate = DateTime.Now,
            //            //FledVoter = false,
            //            //WrongVoter = false,
            //            NotTabulated = false,
            //            TempAddress = (bool)Data.TempAddress.Value,
            //            //LocalOnly = true,
            //            LastSynced = DateTime.Parse("1/1/1"),

            //            // Add voters address
            //            AddressLine1 = Data.Address1,
            //            AddressLine2 = Data.Address2,
            //            City = Data.City,
            //            State = Data.State,
            //            Zip = Data.Zip,
            //            Country = Data.Country,

            //            BallotNumber = Data.BallotNumber,

            //            // Only qualify this field the first time a record is created
            //            CreatedOnDate = DateTime.Now
            //        };

            //        //// Add record to upload queue
            //        //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //        //newVoterSync.SyncId = Guid.NewGuid();
            //        //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //        //newVoterSync.QueueTime = DateTime.Now;

            //        //context.AvVotedRecordSyncs.Add(newVoterSync);

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, newVotedRecord, "Voted at polls", true);

            //        try
            //        {
            //            // Insert new voted record into database
            //            context.AvVotedRecords.Add(newVotedRecord);
            //            //context.Entry(newVotedRecord).State = EntityState.Added;
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        // Check voter status
            //        if (votedRecord.LogCode < 5)
            //        {
            //            // UPDATE EXISTING RECORD

            //            // Update voted record
            //            votedRecord.DateVoted = DateTime.Now;
            //            votedRecord.PrintedDate = DateTime.Now;
            //            votedRecord.PollId = Data.PollID;
            //            votedRecord.Computer = Data.ComputerID;
            //            votedRecord.BallotStyleId = Data.BallotStyleID;
            //            votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //            votedRecord.SignRefused = Data.SignRefused.Value;
            //            votedRecord.UserId = Data.UserId;
            //            votedRecord.LogCode = (int)logCode;
            //            votedRecord.ActivityCode = LogCodeConstants.ConvertToActivity(logCode);
            //            votedRecord.ActivityDate = DateTime.Now;
            //            //votedRecord.FledVoter = false;
            //            //votedRecord.WrongVoter = false;
            //            votedRecord.NotTabulated = false;
            //            //votedRecord.LocalOnly = true;
            //            votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //            votedRecord.BallotNumber = Data.BallotNumber;

            //            //// Add record to upload queue
            //            //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //            //newVoterSync.SyncId = Guid.NewGuid();
            //            //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //            //newVoterSync.QueueTime = DateTime.Now;

            //            // Update Created On Date when NULL
            //            if (votedRecord.CreatedOnDate == DateTime.MinValue)
            //            {
            //                votedRecord.CreatedOnDate = DateTime.Now;
            //            }

            //            //context.AvVotedRecordSyncs.Add(newVoterSync);

            //            // PREPARE UPLOAD
            //            AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //            // CREATE VOTED HISTORY LOG
            //            CreateHistory(context, votedRecord, "Voted at polls");

            //            try
            //            {
            //                // Update the voted record in the database
            //                context.SaveChanges();
            //            }
            //            catch (Exception e)
            //            {
            //                throw e;
            //            }
            //        }
            //        else
            //        {
            //            throw new Exception("This voter has already voted with code: " + votedRecord.LogCode.ToString());
            //        }

            //    } // End null record check

            //} // End using context

        } // End Voted at Polls

        /// <summary>
        /// Spoil an existing ballot
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>

        public void SpoilBallot(int spoiledReasonId)
        {
            SpoilBallot(spoiledReasonId, null, null);
        }

        public void SpoilBallot(int spoiledReasonId, string precinct, int? ballotStyleId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Timestamp no longer used as Primary Key
                //string timestamp = Data.ComputerID.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                // Create voted record template
                AvSpoiled newSpoiledBallot = new AvSpoiled
                {
                    SpoiledId = Guid.NewGuid(),
                    VoterId = Int32.Parse(Data.VoterID),
                    PrintedDate = DateTime.Now,
                    SpoiledReason = spoiledReasonId,
                    //PollId = (int)settings.System.SiteID,
                    PollId = (int)Data.PollID,
                    Computer = Data.ComputerID,
                    UserId = Data.UserId,
                    BallotSurrendered = Data.BallotSurrendered,
                    LocalOnly = true,
                    LastModified = DateTime.Now,
                    BallotNumber = Data.BallotNumber
                };

                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.SpoiledReason = spoiledReasonId;
                    if (ballotStyleId != null)
                    {
                        votedRecord.PrecinctPartId = precinct;
                        votedRecord.BallotStyleId = ballotStyleId;
                        votedRecord.LastModified = DateTime.Now;
                        votedRecord.IgnoreRules = _ForceUpdate;
                        votedRecord.LocalOnly = true;

                        // CREATE VOTED HISTORY LOG
                        CreateHistory(context, votedRecord, "Manually Changed Ballot Style");
                    }                    
                }

                try
                {
                    // Insert new spoiled ballot record into database
                    context.AvSpoileds.Add(newSpoiledBallot);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }

            } // End using context            

        } // End Spoil Ballot

        /// <summary>
        /// Mark a provisional ballot
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void ProvisionalBallot(int provisionalReasonId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Timestamp no longer used as Primary Key
                //string timestamp = Data.ComputerID.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                // Create voted record template
                AvProvisional newProvisionalBallot = new AvProvisional
                {
                    ProvisionalId = Guid.NewGuid(),
                    VoterId = Int32.Parse(Data.VoterID),
                    FirstName = Data.FirstName,
                    MiddleName = Data.MiddleName,
                    LastName = Data.LastName,
                    DateOfBirth = Data.DOBYear,
                    Address = Data.Address1,
                    Address2 = Data.Address2,
                    City = Data.City,
                    State = Data.State,
                    Zip = Data.Zip,
                    BallotStyleId = Data.BallotStyleID,
                    PrintedDate = DateTime.Now,
                    ProvisionalReason = provisionalReasonId,
                    //PollId = (int)settings.System.SiteID,
                    PollId = (int)Data.PollID,
                    Computer = Data.ComputerID,
                    UserId = Data.UserId,
                    LocalOnly = true,
                    LastModified = DateTime.Now
                };

                try
                {
                    // Insert new spoiled ballot record into database
                    context.AvProvisionals.Add(newProvisionalBallot);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }

            } // End using context

        } // End Provisional Ballot

        /// <summary>
        /// Mark Voter as Voided Absentee Ballot
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void VoidedAtPolls(int ElectionMode)
        {
            // Create disposable database context            
            using (var context = GetElectionContext())
            {
                var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)                
                {
                    // Check voter status
                    if (votedRecord.LogCode == 5 || votedRecord.LogCode == 6 || votedRecord.LogCode == 14 || votedRecord.LogCode == 7 || votedRecord.LogCode == 15)
                    {
                        // UPDATE EXISTING RECORD

                        // Update voted record
                        //votedRecord.DateVoted = DateTime.Now;
                        //votedRecord.PrintedDate = DateTime.Now;
                        //votedRecord.PollId = Data.PollID;
                        //votedRecord.Computer = Data.ComputerID;
                        //votedRecord.BallotStyleId = Data.BallotStyleID;
                        //votedRecord.PrecinctPartId = Data.PrecinctPartID;
                        //votedRecord.SignRefused = Data.SignRefused;
                        //votedRecord.UserId = Data.UserId;
                        //votedRecord.LogCode = 2;
                        //votedRecord.ActivityCode = ElectionMode == 2 ? "P" : "E";
                        //votedRecord.ActivityDate = DateTime.Now;
                        //votedRecord.NotTabulated = false;
                        //votedRecord.LastSynced = DateTime.Parse("1/1/1");
                        //votedRecord.ForcePropagate = true;

                        // Force sync ignore the log code rules on this record
                        _ForceUpdate = true;

                        //votedRecord.BallotNumber = Data.BallotNumber;

                        // PREPARE UPLOAD
                        //AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

                        // Create copy of voted record                        
                        //var votedCopy = votedRecord;
                        //votedCopy.LogCode = 2;
                        //votedCopy.PollId = Data.PollID;
                        //votedCopy.Computer = Data.ComputerID;
                        //votedCopy.ActivityDate = DateTime.Now;

                        // CREATE VOTED HISTORY LOG
                        CreateVoidHistory(context, votedRecord, "Voided Absentee Ballot", Data.PollID, Data.ComputerID);

                        try
                        {
                            // Update the voted record in the database
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot void a ballot with code: " + votedRecord.LogCode.ToString());
                    }

                } // End null record check

            } // End using context

        } // End Voided at Polls

        /// <summary>
        /// Update the existing ballot number when a ballot is spoiled
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateBallotNumber()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.BallotNumber = Data.BallotNumber;

                    votedRecord.LastModified = DateTime.Now;
                    votedRecord.IgnoreRules = _ForceUpdate;
                    votedRecord.LocalOnly = true;

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Update ballot number");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Ballot Number

        /// <summary>
        /// Update the existing voted record to Wrong Voter
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateWrongVoter()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    //votedRecord.WrongVoter = true;
                    //votedRecord.FledVoter = false;          // Cannot have both true at the same time
                    votedRecord.NotTabulated = true;
                    votedRecord.ActivityDate = DateTime.Now;
                    //votedRecord.LocalOnly = true;

                    votedRecord.LastModified = DateTime.Now;
                    votedRecord.IgnoreRules = _ForceUpdate;
                    votedRecord.LocalOnly = true;

                    // PREPARE UPLOAD
                    //AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Wrong voter");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Wrong Voter

        /// <summary>
        /// Update the existing voted record to Fled Voter
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateFledVoter()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    //votedRecord.FledVoter = true;
                    //votedRecord.WrongVoter = false; // Cannot have both true at the same time
                    votedRecord.NotTabulated = true;
                    votedRecord.ActivityDate = DateTime.Now;
                    //votedRecord.LocalOnly = true;

                    votedRecord.LastModified = DateTime.Now;
                    votedRecord.IgnoreRules = _ForceUpdate;
                    votedRecord.LocalOnly = true;

                    // PREPARE UPLOAD
                    //AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Fled voter");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Fled Voter

        /// <summary>
        /// Update the existing voted record to Not Tabulated
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateNotTabulated()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.NotTabulated = true;
                    votedRecord.ActivityDate = DateTime.Now;

                    votedRecord.LastModified = DateTime.Now;
                    votedRecord.IgnoreRules = _ForceUpdate;
                    votedRecord.LocalOnly = true;

                    // PREPARE UPLOAD
                    //AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Fled voter");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Not Tabulated

        /// <summary>
        /// Check if any changes have been made to the log code before a ballot is printed
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        // Post check logic added 8/7/2018
        public int? CheckLogCode()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // check if voted Record exists
                try
                {
                    // Get newest log code
                    int? logCode;
                    var serverCopy = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
                    var localCopy = context.AvActivities.Find(Int32.Parse(Data.VoterID));

                    if (localCopy == null && serverCopy == null)
                    {
                        logCode = 0;
                    }
                    else if (serverCopy == null && localCopy != null)
                    {
                        logCode = localCopy.LogCode;
                    }
                    else if (localCopy == null || serverCopy.LastModified > localCopy.LastModified)
                    {
                        logCode = serverCopy.LogCode;
                    }                    
                    else
                    {
                        logCode = localCopy.LogCode;
                    }

                    //logCode = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID)).LogCode;
                    //if (logCode != null) return logCode;
                    //else return 0;
                    return logCode;
                }
                catch (Exception e)
                {
                    throw e;
                    //var test = e;
                    //return 0;
                }

            } // End using context

        } // End Check Log Code

        // Set the Voted Ballot Number to the next highest value from a given site
        public void GetNextBallotNumber(int pollID)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                int next_ballot_number = 0;
                var current_ballot_number = context.AvActivities.Where(rec => rec.PollId == pollID).Max(rec => rec.BallotNumber);
                if (current_ballot_number == null)
                {
                    next_ballot_number = 1;
                }
                else
                {
                    next_ballot_number = (int)current_ballot_number + 1;
                }

                Data.BallotNumber = next_ballot_number;
            }
        }

        // Set the Voter Id to a negative number for voters who have been added a the polls
        // this number should not be site dependant
        public void GetNextProvisionalId(int pollId)
        {
            if (Data.VoterID == null)
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    // Get most recent negative Voter ID
                    var lowest_voter_id = context.AvProvisionals.Where(p => p.VoterId < 1 && p.PollId == pollId).Min(p => p.VoterId);

                    // Set next voter ID
                    if (lowest_voter_id == 0 || lowest_voter_id == null)
                    {
                        Data.VoterID = "-1";
                    }
                    else
                    {
                        Data.VoterID = ((int)(lowest_voter_id - 1)).ToString();
                    }
                }
            }
        }

        public List<VoterHistoryModel> HistoryList()
        {
            using (var context = GetElectionContext())
            {
                return context.AvVotedRecordHistorys.Where(vrh => vrh.VoterId.ToString() == Data.VoterID)
                    .Select(h => new VoterHistoryModel()
                    {
                        VotedRecordHistoryId = h.VotedRecordHistoryId,
                        HistoryDate = h.HistoryDate,
                        HistoryAction = h.HistoryAction,
                        HistoryInit = h.HistoryInit,
                        VoterId = h.VoterId,
                        LogCode = h.LogCode,
                        ElectionId = h.ElectionId,
                        PrecinctPartId = h.PrecinctPartId,
                        BallotStyleId = h.BallotStyleId,
                        PollId = h.PollId,
                        LastSynced = h.LastSynced,
                        CountyCode = h.CountyCode,
                        ApplicationIssued = h.ApplicationIssued,
                        ApplicationAccepted = h.ApplicationAccepted,
                        ApplicationRejected = h.ApplicationRejected,
                        DateIssued = h.DateIssued,
                        PrintedDate = h.PrintedDate,
                        AbsenteeId = h.AbsenteeId,
                        AbsenteeVoterType = h.AbsenteeVoterType,
                        ApplicationSource = h.ApplicationSource,
                        UocavaApplicationType = h.UocavaApplicationType,
                        UocavaVoterEmail = h.UocavaVoterEmail,
                        UocavaVoterFax = h.UocavaVoterFax,
                        BallotDeliveryMethod = h.BallotDeliveryMethod,
                        AddressType = h.AddressType,
                        AddressLine1 = h.AddressLine1,
                        AddressLine2 = h.AddressLine2,
                        City = h.City,
                        State = h.State,
                        Zip = h.Zip,
                        Country = h.Country,
                        TempAddress = h.TempAddress,
                        DateVoted = h.DateVoted,
                        Computer = h.Computer,
                        BallotNumber = h.BallotNumber,
                        SignRefused = h.SignRefused,
                        LocalOnly = h.LocalOnly,
                        UserId = h.UserId,
                        ActivityCode = h.ActivityCode,
                        ActivityDate = h.ActivityDate,
                        ProvisionalOnly = h.ProvisionalOnly,
                        NotTabulated = h.NotTabulated,
                        AppRejectedReason = h.AppRejectedReason,
                        SpoiledReason = h.SpoiledReason,
                        BcMailDate = h.BcMailDate,
                        ForcePropagate = h.ForcePropagate,
                        CreatedOnDate = h.CreatedOnDate,
                        IMBOut = h.IMBOut,
                        IMBIn = h.IMBIn,
                        VoterContactEmail = h.VoterContactEmail,
                        VoterContactPhone = h.VoterContactPhone,
                        BallotRejectedDate = h.BallotRejectedDate,
                        BallotRejectedReason = h.BallotRejectedReason,
                        ServisABSLastModified = h.ServisABSLastModified,
                        ServisVotedLastModified = h.ServisVotedLastModified
                    })
                    .ToList();
            }
        }

        public void RestoreHistory(Guid HistoryId)
        {
            using (var context = GetElectionContext())
            {
                var VoterHistory = context.AvVotedRecordHistorys.Where(vrh => vrh.VotedRecordHistoryId == HistoryId && vrh.VoterId.ToString() == Data.VoterID).FirstOrDefault();

                //VotedRecordHistoryId = Guid.NewGuid(),
                //HistoryDate = DateTime.Now,
                //HistoryAction = action,
                //HistoryInit = init,
                //VoterId = voter.VoterId,
                Data.LogCode = VoterHistory.LogCode;
                Data.ElectionID = VoterHistory.ElectionId;
                Data.PrecinctPartID = VoterHistory.PrecinctPartId;
                Data.BallotStyleID = VoterHistory.BallotStyleId;
                Data.PollID = VoterHistory.PollId;
                //Data.LastSynced = VoterHistory.LastSynced;
                Data.County = VoterHistory.CountyCode;
                Data.ApplicationIssued = VoterHistory.ApplicationIssued;
                Data.ApplicationAccepted = VoterHistory.ApplicationAccepted;
                Data.ApplicationRejected = VoterHistory.ApplicationRejected;
                Data.BallotIssued = VoterHistory.DateIssued;
                Data.BallotPrinted = VoterHistory.PrintedDate;
                Data.AbsenteeId = VoterHistory.AbsenteeId;
                Data.AbsenteeType = VoterHistory.AbsenteeVoterType;
                Data.ApplicationSource = VoterHistory.ApplicationSource;
                Data.UocavaApplicationType = VoterHistory.UocavaApplicationType;
                Data.UocavaVoterEmail = VoterHistory.UocavaVoterEmail;
                Data.UocavaVoterFax = VoterHistory.UocavaVoterFax;
                Data.BallotDeliveryMethod = VoterHistory.BallotDeliveryMethod;
                Data.AddressType = VoterHistory.AddressType;
                Data.DeliveryAddress1 = VoterHistory.AddressLine1;
                Data.DeliveryAddress2 = VoterHistory.AddressLine2;
                Data.DeliveryCity = VoterHistory.City;
                Data.DeliveryState = VoterHistory.State;
                Data.DeliveryZip = VoterHistory.Zip;
                Data.DeliveryCountry = VoterHistory.Country;
                Data.TempAddress = VoterHistory.TempAddress;
                Data.VotedDate = VoterHistory.DateVoted;
                Data.ComputerID = VoterHistory.Computer;
                Data.BallotNumber = VoterHistory.BallotNumber;
                Data.SignRefused = VoterHistory.SignRefused;
                //LocalOnly = true;
                Data.UserId = VoterHistory.UserId;
                Data.ActivityCode = VoterHistory.ActivityCode;
                Data.ActivityDate = VoterHistory.ActivityDate;
                Data.ProvisionalOnly = VoterHistory.ProvisionalOnly;
                //FledVoter = voter.FledVoter,
                //WrongVoter = voter.WrongVoter,
                Data.NotTabulated = VoterHistory.NotTabulated;
                Data.ApplicationRejectedReason = VoterHistory.AppRejectedReason;
                Data.SpoiledReason = VoterHistory.SpoiledReason;
                Data.BcMailDate = VoterHistory.BcMailDate;
                Data.CreatedOnDate = VoterHistory.CreatedOnDate;
                Data.OutGoingIMB = VoterHistory.IMBOut;
                Data.InComingIMB = VoterHistory.IMBIn;
                Data.Email = VoterHistory.VoterContactEmail;
                Data.Phone = VoterHistory.VoterContactPhone;
                Data.BallotRejectedDate = VoterHistory.BallotRejectedDate;
                Data.BallotRejectedReason = VoterHistory.BallotRejectedReason;
                Data.ServisABSLastModified = VoterHistory.ServisABSLastModified;
                Data.ServisVotedLastModified = VoterHistory.ServisVotedLastModified;
            }
        }

        //public void ChangeStatus(int Status)
        //{
        //    if(Status < 8)
        //    {
        //        // Reset Dates
        //        if (Data.VotedDate != null) Data.VotedDate = null;
        //        if (Data.BallotPrinted != null) Data.BallotPrinted = null;
        //        if (Data.ActivityCode != null) Data.ActivityCode = null;

        //        // Reset Created On Date
        //        if (Data.LogCode > 5) Data.CreatedOnDate = DateTime.MinValue;

        //        Data.ActivityDate = DateTime.Now;
        //        Data.LogCode = Status;
        //    }
        //    else
        //    {
        //        throw new Exception("Invalid Status");
        //    }
        //}

        public void SaveChanges(string ChangeReason)
        {
            // Force record to Ignore Log Code Rules
            _ForceUpdate = true;

            UpdateVoted(ChangeReason, true);
        }

        #endregion

        #region ABSMethods

        /// <summary>
        /// Mark Voter with details required for issuing an application
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void IssueApplication()
        {
            // Check if a newer log code was downloaded from the server
            var logCode = CheckLogCode();
            if (logCode < 5 || logCode == null)
            {
                Data.LogCode = LogCodeConstants.IssuedApplication;
                Data.ActivityDate = DateTime.Now;
                Data.NotTabulated = false;

                // Delivery Address should already be set
                //Data.DeliveryAddress1 = Data.Address1;
                //Data.DeliveryAddress2 = Data.Address2;
                //Data.DeliveryCity = Data.City;
                //Data.DeliveryState = Data.State;
                //Data.DeliveryZip = Data.Zip;
                //Data.DeliveryCountry = Data.Country;

                // Set address type
                if (Data.DeliveryCountry == null || Data.DeliveryCountry == "") 
                {
                    Data.AddressType = "D";
                }
                else
                {
                    Data.AddressType = "F";
                }

                UpdateVoted("Issued application");
            }
            else
            {
                throw new Exception("This voter has already voted with code: " + Data.LogCode.ToString());
            }

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord == null)
            //    {
            //        // INSERT NEW VOTED RECORD

            //        // Create voted record template
            //        AvVotedRecord newAbsenteeRecord = new AvVotedRecord
            //        {
            //            // Map fields to template
            //            VoterId = Int32.Parse(Data.VoterID),
            //            CountyCode = Data.County,
            //            ElectionId = Data.ElectionID,
            //            PollId = Data.PollID,
            //            Computer = Data.ComputerID,
            //            BallotStyleId = Data.BallotStyleID,
            //            PrecinctPartId = Data.PrecinctPartID,
            //            UserId = Data.UserId,
            //            LogCode = LogCodeConstants.IssuedApplication,
            //            //ActivityCode = settings.System.VCCType == 2 ? "P" : "E",
            //            ActivityDate = DateTime.Now,
            //            //FledVoter = false,
            //            //WrongVoter = false,
            //            NotTabulated = false,
            //            //LocalOnly = true,
            //            LastSynced = DateTime.Parse("1/1/1"),

            //            // Add voters address
            //            AddressLine1 = Data.DeliveryAddress1,
            //            AddressLine2 = Data.DeliveryAddress2,
            //            City = Data.DeliveryCity,
            //            State = Data.DeliveryState,
            //            Zip = Data.DeliveryZip,
            //            Country = Data.DeliveryCountry,
            //            TempAddress = Data.TempAddress.Value,

            //            BallotNumber = Data.BallotNumber,

            //            ApplicationIssued = Data.ApplicationIssued,
            //            AbsenteeVoterType = Data.AbsenteeType,

            //            VoterContactPhone = Data.Phone,
            //            VoterContactEmail = Data.Email,

            //            // Only qualify this field the first time a record is created
            //            CreatedOnDate = DateTime.MinValue

            //        };

            //        // Set address type
            //        if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        {
            //            newAbsenteeRecord.AddressType = "D";
            //        }
            //        else
            //        {
            //            newAbsenteeRecord.AddressType = "F";
            //        }

            //        //// Add record to upload queue
            //        //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //        //newVoterSync.SyncId = Guid.NewGuid();
            //        //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //        //newVoterSync.QueueTime = DateTime.Now;

            //        //context.AvVotedRecordSyncs.Add(newVoterSync);

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, newAbsenteeRecord, "Issued application", true);

            //        try
            //        {
            //            // Insert new voted record into database
            //            context.AvVotedRecords.Add(newAbsenteeRecord);
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        // Check voter status
            //        if (votedRecord.LogCode < 5)
            //        {
            //            // UPDATE EXISTING RECORD

            //            // Update voted record
            //            votedRecord.PollId = Data.PollID;
            //            votedRecord.Computer = Data.ComputerID;
            //            votedRecord.BallotStyleId = Data.BallotStyleID;
            //            votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //            votedRecord.UserId = Data.UserId;
            //            votedRecord.LogCode = LogCodeConstants.IssuedApplication;
            //            //votedRecord.ActivityCode = settings.System.VCCType == 2 ? "P" : "A";
            //            votedRecord.ActivityDate = DateTime.Now;
            //            //votedRecord.FledVoter = false;
            //            //votedRecord.WrongVoter = false;
            //            votedRecord.NotTabulated = false;
            //            //votedRecord.LocalOnly = true;
            //            votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //            // Update voters address
            //            votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //            votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //            votedRecord.City = Data.DeliveryCity;
            //            votedRecord.State = Data.DeliveryState;
            //            votedRecord.Zip = Data.DeliveryZip;
            //            votedRecord.Country = Data.DeliveryCountry;
            //            votedRecord.TempAddress = Data.TempAddress.Value;

            //            votedRecord.BallotNumber = Data.BallotNumber;

            //            votedRecord.ApplicationIssued = Data.ApplicationIssued;
            //            votedRecord.AbsenteeVoterType = Data.AbsenteeType;

            //            // Set address type
            //            if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //            {
            //                votedRecord.AddressType = "D";
            //            }
            //            else
            //            {
            //                votedRecord.AddressType = "F";
            //            }

            //            //// Add record to upload queue
            //            //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //            //newVoterSync.SyncId = Guid.NewGuid();
            //            //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //            //newVoterSync.QueueTime = DateTime.Now;

            //            //context.AvVotedRecordSyncs.Add(newVoterSync);

            //            votedRecord.VoterContactPhone = Data.Phone;
            //            votedRecord.VoterContactEmail = Data.Email;

            //            // PREPARE UPLOAD
            //            AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //            // CREATE VOTED HISTORY LOG
            //            CreateHistory(context, votedRecord, "Issued application");

            //            try
            //            {
            //                // Update the voted record in the database
            //                context.SaveChanges();
            //            }
            //            catch (Exception e)
            //            {
            //                throw e;
            //            }
            //        }
            //        else
            //        {
            //            throw new Exception("This voter has already voted with code: " + votedRecord.LogCode.ToString());
            //        }

            //    } // End null record check

            //} // End using context

        } // End Issue Application

        /// <summary>
        /// Update the existing voted address
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateAddress()
        {

            UpdateVoted("Address Updated");

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    // Get existing Voted Record
            //    var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord != null)
            //    {

            //        votedRecord.ActivityDate = DateTime.Now;
            //        //votedRecord.LocalOnly = true;
            //        votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //        // Mailing Address
            //        votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //        votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //        votedRecord.City = Data.DeliveryCity;
            //        votedRecord.State = Data.DeliveryState;
            //        votedRecord.Zip = Data.DeliveryZip;
            //        votedRecord.Country = Data.DeliveryCountry;
            //        votedRecord.TempAddress = Data.TempAddress.Value;

            //        //if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        //{
            //        //    votedRecord.AddressType = "D";
            //        //}
            //        //else
            //        //{
            //        //    votedRecord.AddressType = "F";
            //        //}

            //        votedRecord.AbsenteeVoterType = Data.AbsenteeType;

            //        // PREPARE UPLOAD
            //        //AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, votedRecord, "Change address");

            //        try
            //        {
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
            //    }

            //} // End using context

        } // End Update Address

        /// <summary>
        /// Accepts existing application or issues and accepts a new application
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void AcceptApplication()
        {
            Data.LogCode = LogCodeConstants.AcceptedApplication;
            Data.ActivityDate = DateTime.Now;
            Data.NotTabulated = false;

            Data.ApplicationSource = "M";

            // Set address type
            if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            {
                Data.AddressType = "D";
            }
            else
            {
                Data.AddressType = "F";
            }

            if (Data.CreatedOnDate == DateTime.MinValue)
            {
                Data.CreatedOnDate = DateTime.Now;
            }

            UpdateVoted("Accept application");

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    // Get existing Voted Record
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord == null)
            //    {
            //        // INSERT NEW VOTED RECORD

            //        // Create voted record template
            //        AvVotedRecord newAbsenteeRecord = new AvVotedRecord
            //        {

            //            // Map fields to template
            //            VoterId = Int32.Parse(Data.VoterID),
            //            CountyCode = Data.County,
            //            ElectionId = Data.ElectionID,
            //            PollId = Data.PollID,
            //            Computer = Data.ComputerID,
            //            BallotStyleId = Data.BallotStyleID,
            //            PrecinctPartId = Data.PrecinctPartID,
            //            UserId = Data.UserId,
            //            LogCode = LogCodeConstants.AcceptedApplication,
            //            //ActivityCode = settings.System.VCCType == 2 ? "P" : "E",
            //            ActivityDate = DateTime.Now,
            //            //FledVoter = false,
            //            //WrongVoter = false,
            //            NotTabulated = false,
            //            //LocalOnly = true,
            //            LastSynced = DateTime.Parse("1/1/1"),

            //            // Add voters address
            //            AddressLine1 = Data.DeliveryAddress1,
            //            AddressLine2 = Data.DeliveryAddress2,
            //            City = Data.DeliveryCity,
            //            State = Data.DeliveryState,
            //            Zip = Data.DeliveryZip,
            //            Country = Data.DeliveryCountry,
            //            TempAddress = Data.TempAddress.Value,

            //            BallotNumber = Data.BallotNumber,

            //            ApplicationAccepted = Data.ApplicationAccepted,
            //            ApplicationSource = "M",
            //            ApplicationIssued = Data.ApplicationIssued,
            //            AbsenteeVoterType = Data.AbsenteeType,

            //            VoterContactPhone = Data.Phone,
            //            VoterContactEmail = Data.Email,

            //            // Only qualify this field the first time a record is created
            //            CreatedOnDate = DateTime.MinValue

            //        };

            //        // Set address type
            //        if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        {
            //            newAbsenteeRecord.AddressType = "D";
            //        }
            //        else
            //        {
            //            newAbsenteeRecord.AddressType = "F";
            //        }

            //        //// Add record to upload queue
            //        //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //        //newVoterSync.SyncId = Guid.NewGuid();
            //        //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //        //newVoterSync.QueueTime = DateTime.Now;

            //        //context.AvVotedRecordSyncs.Add(newVoterSync);

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, newAbsenteeRecord, "Accept application", true);

            //        try
            //        {
            //            // Insert new voted record into database
            //            context.AvVotedRecords.Add(newAbsenteeRecord);
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        votedRecord.ApplicationAccepted = Data.ApplicationAccepted;
            //        votedRecord.ApplicationSource = "M";
            //        votedRecord.PollId = Data.PollID;
            //        votedRecord.Computer = Data.ComputerID;
            //        votedRecord.BallotStyleId = Data.BallotStyleID;
            //        votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //        votedRecord.UserId = Data.UserId;
            //        votedRecord.LogCode = LogCodeConstants.AcceptedApplication;
            //        votedRecord.ActivityDate = DateTime.Now;
            //        //votedRecord.LocalOnly = true;
            //        votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //        votedRecord.VoterContactPhone = Data.Phone;
            //        votedRecord.VoterContactEmail = Data.Email;

            //        // Mailing Address
            //        votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //        votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //        votedRecord.City = Data.DeliveryCity;
            //        votedRecord.State = Data.DeliveryState;
            //        votedRecord.Zip = Data.DeliveryZip;
            //        votedRecord.Country = Data.DeliveryCountry;
            //        votedRecord.TempAddress = Data.TempAddress.Value;

            //        //if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        //{
            //        //    votedRecord.AddressType = "D";
            //        //}
            //        //else
            //        //{
            //        //    votedRecord.AddressType = "F";
            //        //}

            //        votedRecord.AbsenteeVoterType = Data.AbsenteeType;

            //        //// Add record to upload queue
            //        //AvVotedRecordSync newVoterSync = new AvVotedRecordSync();
            //        //newVoterSync.SyncId = Guid.NewGuid();
            //        //newVoterSync.VoterId = Int32.Parse(Data.VoterID);
            //        //newVoterSync.QueueTime = DateTime.Now;

            //        //context.AvVotedRecordSyncs.Add(newVoterSync);

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, votedRecord, "Accept application");

            //        try
            //        {
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }

            //} // End using context

        } // End Accept Application

        /// <summary>
        /// Reject existing application
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void RejectApplication(string rejectedReason)
        {
            Data.LogCode = LogCodeConstants.RejectedApplication;
            Data.ActivityDate = DateTime.Now;
            Data.NotTabulated = false;

            Data.ApplicationSource = "M";

            // Set address type
            if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            {
                Data.AddressType = "D";
            }
            else
            {
                Data.AddressType = "F";
            }

            UpdateVoted("Reject application");

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    // Get existing Voted Record
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord != null)
            //    {
            //        votedRecord.ApplicationRejected = Data.ApplicationRejected;
            //        votedRecord.AppRejectedReason = rejectedReason;
            //        votedRecord.ApplicationSource = "M";
            //        votedRecord.PollId = Data.PollID;
            //        votedRecord.Computer = Data.ComputerID;
            //        votedRecord.BallotStyleId = Data.BallotStyleID;
            //        votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //        votedRecord.UserId = Data.UserId;
            //        votedRecord.LogCode = LogCodeConstants.RejectedApplication;
            //        votedRecord.ActivityDate = DateTime.Now;
            //        //votedRecord.LocalOnly = true;
            //        votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //        votedRecord.VoterContactPhone = Data.Phone;
            //        votedRecord.VoterContactEmail = Data.Email;

            //        // Mailing Address
            //        //votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //        //votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //        //votedRecord.City = Data.DeliveryCity;
            //        //votedRecord.State = Data.DeliveryState;
            //        //votedRecord.Zip = Data.DeliveryZip;
            //        //votedRecord.Country = Data.DeliveryCountry;

            //        //if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        //{
            //        //    votedRecord.AddressType = "D";
            //        //}
            //        //else
            //        //{
            //        //    votedRecord.AddressType = "F";
            //        //}

            //        votedRecord.AbsenteeVoterType = Data.AbsenteeType;

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, votedRecord, "Reject application");

            //        try
            //        {
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
            //    }

            //} // End using context

        } // End Reject Application

        public void RejectBallot(string rejectedReason)
        {
            Data.LogCode = LogCodeConstants.UnsignedAbsenteeBallot;
            Data.ActivityDate = DateTime.Now;
            Data.NotTabulated = false;

            Data.BallotRejectedDate = DateTime.Now;

            UpdateVoted("Reject ballot");

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    // Get existing Voted Record
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord != null)
            //    {
            //        votedRecord.BallotRejectedDate = DateTime.Now;
            //        votedRecord.BallotRejectedReason = rejectedReason;
            //        votedRecord.ApplicationSource = "M";
            //        votedRecord.PollId = Data.PollID;
            //        votedRecord.Computer = Data.ComputerID;
            //        votedRecord.BallotStyleId = Data.BallotStyleID;
            //        votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //        votedRecord.UserId = Data.UserId;
            //        votedRecord.LogCode = LogCodeConstants.UnsignedAbsenteeBallot;
            //        votedRecord.ActivityDate = DateTime.Now;
            //        //votedRecord.LocalOnly = true;
            //        votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //        // Mailing Address
            //        //votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //        //votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //        //votedRecord.City = Data.DeliveryCity;
            //        //votedRecord.State = Data.DeliveryState;
            //        //votedRecord.Zip = Data.DeliveryZip;
            //        //votedRecord.Country = Data.DeliveryCountry;

            //        //if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        //{
            //        //    votedRecord.AddressType = "D";
            //        //}
            //        //else
            //        //{
            //        //    votedRecord.AddressType = "F";
            //        //}

            //        votedRecord.AbsenteeVoterType = Data.AbsenteeType;

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, votedRecord, "Reject ballot");

            //        try
            //        {
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
            //    }

            //} // End using context

        } // End Reject Ballot

        /// <summary>
        /// Accepts existing application and issues an official ballot
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void IssueBallot()
        {
            // Get Election record
            var electionRecord = GetElection();

            DateTime todaysDate = DateTime.Now;

            // Chenge voted date if today is before Absentee start
            if (electionRecord != null && electionRecord.AbsenteeBeginDate != null)
            {
                if (todaysDate < electionRecord.AbsenteeBeginDate) todaysDate = (DateTime)electionRecord.AbsenteeBeginDate;
            }

            if (Data.ApplicationIssued == null) Data.ApplicationIssued = DateTime.Now;
            if (Data.ApplicationAccepted == null) Data.ApplicationAccepted = DateTime.Now;
            Data.ApplicationSource = "M";
            Data.BallotDeliveryMethod = "M";
            Data.LogCode = LogCodeConstants.IssuedAbsenteeBallot;
            Data.ActivityDate = todaysDate;
            Data.NotTabulated = false;

            Data.BallotIssued = todaysDate;
            Data.BallotPrinted = todaysDate;

            // Set address type
            if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            {
                Data.AddressType = "D";
            }
            else
            {
                Data.AddressType = "F";
            }

            if (Data.CreatedOnDate == DateTime.MinValue)
            {
                Data.CreatedOnDate = DateTime.Now;
            }

            UpdateVoted("Issue ballot");

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    // Get existing Voted Record
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));

            //    // Get Election record
            //    var electionRecord = context.SosElections.Find(Data.ElectionID);
                
            //    DateTime todaysDate = DateTime.Now;

            //    // Chenge voted date if today is before Absentee start
            //    if (electionRecord != null && electionRecord.AbsenteeBeginDate != null)
            //    {
            //        if (todaysDate < electionRecord.AbsenteeBeginDate) todaysDate = (DateTime)electionRecord.AbsenteeBeginDate;
            //    }

            //    if (votedRecord == null)
            //    {
            //        // INSERT NEW VOTED RECORD

            //        // Create voted record template
            //        AvVotedRecord newAbsenteeRecord = new AvVotedRecord
            //        {

            //            // Map fields to template
            //            VoterId = Int32.Parse(Data.VoterID),
            //            CountyCode = Data.County,
            //            ElectionId = Data.ElectionID,
            //            PollId = Data.PollID,
            //            Computer = Data.ComputerID,
            //            BallotStyleId = Data.BallotStyleID,
            //            PrecinctPartId = Data.PrecinctPartID,
            //            UserId = Data.UserId,
            //            LogCode = LogCodeConstants.IssuedAbsenteeBallot,
            //            //ActivityCode = settings.System.VCCType == 2 ? "P" : "E",
            //            ActivityCode = "A",
            //            ActivityDate = todaysDate,
            //            //FledVoter = false,
            //            //WrongVoter = false,
            //            NotTabulated = false,
            //            //LocalOnly = true,
            //            LastSynced = DateTime.Parse("1/1/1"),

            //            // Add voters address
            //            AddressLine1 = Data.DeliveryAddress1,
            //            AddressLine2 = Data.DeliveryAddress2,
            //            City = Data.DeliveryCity,
            //            State = Data.DeliveryState,
            //            Zip = Data.DeliveryZip,
            //            Country = Data.DeliveryCountry,
            //            TempAddress = Data.TempAddress.Value,

            //            BallotNumber = Data.BallotNumber,

            //            // Issue and accept application
            //            ApplicationAccepted = Data.ApplicationAccepted,
            //            ApplicationSource = "M",
            //            BallotDeliveryMethod = "M",
            //            ApplicationIssued = Data.ApplicationIssued,
            //            AbsenteeVoterType = Data.AbsenteeType,
            //            DateIssued = todaysDate,
            //            PrintedDate = todaysDate,

            //            // Intelligent Barcodes
            //            IMBOut = Data.OutGoingIMB,
            //            IMBIn = Data.InComingIMB,

            //            VoterContactPhone = Data.Phone,
            //            VoterContactEmail = Data.Email,

            //            // Only set this field the first time a record is created
            //            CreatedOnDate = DateTime.Now

            //        };

            //        // Set address type
            //        if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        {
            //            newAbsenteeRecord.AddressType = "D";
            //        }
            //        else
            //        {
            //            newAbsenteeRecord.AddressType = "F";
            //        }

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, newAbsenteeRecord, "Issue ballot", true);

            //        try
            //        {
            //            // Insert new voted record into database
            //            context.AvVotedRecords.Add(newAbsenteeRecord);
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        if (votedRecord.ApplicationIssued == null) votedRecord.ApplicationIssued = DateTime.Now;
            //        if (votedRecord.ApplicationAccepted == null) votedRecord.ApplicationAccepted = DateTime.Now;
            //        votedRecord.ApplicationSource = "M";
            //        votedRecord.BallotDeliveryMethod = "M";
            //        votedRecord.PollId = Data.PollID;
            //        votedRecord.Computer = Data.ComputerID;
            //        votedRecord.BallotStyleId = Data.BallotStyleID;
            //        votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //        votedRecord.UserId = Data.UserId;
            //        votedRecord.LogCode = LogCodeConstants.IssuedAbsenteeBallot;
            //        votedRecord.ActivityDate = DateTime.Now;
            //        votedRecord.ActivityCode = "A";
            //        //votedRecord.LocalOnly = true;
            //        votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //        // Mailing Address
            //        votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //        votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //        votedRecord.City = Data.DeliveryCity;
            //        votedRecord.State = Data.DeliveryState;
            //        votedRecord.Zip = Data.DeliveryZip;
            //        votedRecord.Country = Data.DeliveryCountry;
            //        votedRecord.TempAddress = Data.TempAddress.Value;
                    
            //        votedRecord.DateIssued = todaysDate;
            //        votedRecord.PrintedDate = todaysDate;

            //        votedRecord.BallotNumber = Data.BallotNumber;

            //        votedRecord.VoterContactEmail = Data.Email;
            //        votedRecord.VoterContactPhone = Data.Phone;

            //        votedRecord.BallotRejectedDate = null;
            //        votedRecord.BallotRejectedReason = null;

            //        if (Data.DeliveryCountry == null || Data.DeliveryCountry == "")
            //        {
            //            votedRecord.AddressType = "D";
            //        }
            //        else
            //        {
            //            votedRecord.AddressType = "F";
            //        }

            //        votedRecord.AbsenteeVoterType = Data.AbsenteeType;

            //        // Intelligent Barcodes
            //        votedRecord.IMBOut = Data.OutGoingIMB;
            //        votedRecord.IMBIn = Data.InComingIMB;

            //        // Update Created On Date when NULL
            //        if (votedRecord.CreatedOnDate == DateTime.MinValue)
            //        {
            //            votedRecord.CreatedOnDate = DateTime.Now;
            //        }

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, votedRecord, "Issue ballot");

            //        try
            //        {
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }

            //} // End using context

        } // End Accept Application

        /// <summary>
        /// Mark Voter as Voted in Person
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void VotedInPerson()
        {
            Data.VotedDate = DateTime.Now;
            Data.BallotPrinted = DateTime.Now;
            Data.LogCode = LogCodeConstants.InPerson;
            Data.ActivityCode = "E";
            Data.ActivityDate = DateTime.Now;
            Data.NotTabulated = false;

            // Add voters address
            Data.DeliveryAddress1 = Data.Address1;
            Data.DeliveryAddress2 = Data.Address2;
            Data.DeliveryCity = Data.City;
            Data.DeliveryState = Data.State;
            Data.DeliveryZip = Data.Zip;
            Data.DeliveryCountry = Data.Country;

            Data.BallotRejectedDate = null;
            Data.BallotRejectedReason = null;

            if (Data.CreatedOnDate == DateTime.MinValue)
            {
                Data.CreatedOnDate = DateTime.Now;
            }

            UpdateVoted("Voted in person");

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord == null)
            //    {
            //        // INSERT NEW VOTED RECORD

            //        // Create voted record template
            //        AvVotedRecord newVotedRecord = new AvVotedRecord
            //        {
            //            // Map fields to template
            //            VoterId = Int32.Parse(Data.VoterID),
            //            CountyCode = Data.County,
            //            ElectionId = Data.ElectionID,
            //            DateVoted = DateTime.Now,
            //            PrintedDate = DateTime.Now,
            //            PollId = Data.PollID,
            //            Computer = Data.ComputerID,
            //            BallotStyleId = Data.BallotStyleID,
            //            PrecinctPartId = Data.PrecinctPartID,
            //            SignRefused = Data.SignRefused.Value,
            //            UserId = Data.UserId,
            //            LogCode = LogCodeConstants.InPerson,
            //            ActivityCode = "E",
            //            ActivityDate = DateTime.Now,
            //            //FledVoter = false,
            //            //WrongVoter = false,
            //            NotTabulated = false,
            //            //LocalOnly = true,
            //            LastSynced = DateTime.Parse("1/1/1"),

            //            // Add voters address
            //            AddressLine1 = Data.Address1,
            //            AddressLine2 = Data.Address2,
            //            City = Data.City,
            //            State = Data.State,
            //            Zip = Data.Zip,
            //            Country = Data.Country,

            //            BallotNumber = Data.BallotNumber,

            //            VoterContactPhone = Data.Phone,
            //            VoterContactEmail = Data.Email,

            //            // Only qualify this field the first time a record is created
            //            CreatedOnDate = DateTime.Now
            //        };

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, newVotedRecord, "Voted in person", true);

            //        try
            //        {
            //            // Insert new voted record into database
            //            context.AvVotedRecords.Add(newVotedRecord);
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        // Check voter status
            //        if (votedRecord.LogCode < 5)
            //        {
            //            // UPDATE EXISTING RECORD

            //            // Update voted record
            //            votedRecord.DateVoted = DateTime.Now;
            //            votedRecord.PrintedDate = DateTime.Now;
            //            votedRecord.PollId = Data.PollID;
            //            votedRecord.Computer = Data.ComputerID;
            //            votedRecord.BallotStyleId = Data.BallotStyleID;
            //            votedRecord.PrecinctPartId = Data.PrecinctPartID;
            //            votedRecord.SignRefused = Data.SignRefused.Value;
            //            votedRecord.UserId = Data.UserId;
            //            votedRecord.LogCode = LogCodeConstants.InPerson;
            //            votedRecord.ActivityCode = "E";
            //            votedRecord.ActivityDate = DateTime.Now;
            //            //votedRecord.FledVoter = false;
            //            //votedRecord.WrongVoter = false;
            //            votedRecord.NotTabulated = false;
            //            //votedRecord.LocalOnly = true;
            //            votedRecord.LastSynced = DateTime.Parse("1/1/1");

            //            votedRecord.BallotNumber = Data.BallotNumber;

            //            votedRecord.VoterContactEmail = Data.Email;
            //            votedRecord.VoterContactPhone = Data.Phone;

            //            votedRecord.BallotRejectedDate = null;
            //            votedRecord.BallotRejectedReason = null;

            //            // Update Created On Date when NULL
            //            if (votedRecord.CreatedOnDate == DateTime.MinValue)
            //            {
            //                votedRecord.CreatedOnDate = DateTime.Now;
            //            }

            //            // PREPARE UPLOAD
            //            AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //            // CREATE VOTED HISTORY LOG
            //            CreateHistory(context, votedRecord, "Voted in person");

            //            try
            //            {
            //                // Update the voted record in the database
            //                context.SaveChanges();
            //            }
            //            catch (Exception e)
            //            {
            //                throw e;
            //            }
            //        }
            //        else
            //        {
            //            throw new Exception("This voter has already voted with code: " + votedRecord.LogCode.ToString());
            //        }

            //    } // End null record check

            //} // End using context

        } // End Voted at Polls

        /// <summary>
        /// Mark return ballot with the given log code. Accpetable codes are 9, 10, 12 and 14.
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void ReturnBallot(int code)
        {
            // https://stackoverflow.com/questions/29482/cast-int-to-enum-in-c-sharp
            this.ReturnBallot(
                (LogCodes)Enum.ToObject(typeof(LogCodes), code)
                );
        }
        public void ReturnBallot(LogCodes ballotAcceptedCode)
        {
            var acceptableCodes = new List<int> { 9, 12, 14, 15 };
            if (acceptableCodes.Contains((int)ballotAcceptedCode))
            {
                Data.LogCode = (int)ballotAcceptedCode;
                if ((int)ballotAcceptedCode == 9) Data.ActivityCode = "A";
                Data.ActivityDate = DateTime.Now;
                Data.NotTabulated = false;

                if (ballotAcceptedCode == LogCodes.ReturnedAbsenteeBallot ||
                            ballotAcceptedCode == LogCodes.UnsignedAbsenteeBallot)
                {
                    Data.VotedDate = DateTime.Now;
                }

                Data.BallotRejectedDate = null;
                Data.BallotRejectedReason = null;

                if (Data.CreatedOnDate == DateTime.MinValue)
                {
                    Data.CreatedOnDate = DateTime.Now;
                }

                UpdateVoted("Returned ballot");

                //// Create disposable database context
                //using (var context = GetElectionContext())
                //{
                //    // Get existing Voted Record
                //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
                //    if (votedRecord != null)
                //    {
                //        votedRecord.PollId = Data.PollID;
                //        votedRecord.Computer = Data.ComputerID;
                //        votedRecord.BallotStyleId = Data.BallotStyleID;
                //        votedRecord.PrecinctPartId = Data.PrecinctPartID;
                //        votedRecord.UserId = Data.UserId;
                //        votedRecord.LogCode = (int)ballotAcceptedCode;
                //        if ((int)ballotAcceptedCode == 9) votedRecord.ActivityCode = "A";
                //        //votedRecord.ActivityDate = DateTime.Now;
                //        votedRecord.ActivityDate = Data.ActivityDate;
                //        //votedRecord.LocalOnly = true;
                //        votedRecord.LastSynced = DateTime.Parse("1/1/1");

                //        if(ballotAcceptedCode == LogCodes.ReturnedAbsenteeBallot ||
                //            ballotAcceptedCode == LogCodes.UnsignedAbsenteeBallot)
                //        {
                //            votedRecord.DateVoted = DateTime.Now;
                //        }

                //        votedRecord.BallotRejectedDate = null;
                //        votedRecord.BallotRejectedReason = null;

                //        // PREPARE UPLOAD
                //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

                //        // CREATE VOTED HISTORY LOG
                //        CreateHistory(context, votedRecord, "Returned ballot");

                //        try
                //        {
                //            context.SaveChanges();
                //        }
                //        catch (Exception e)
                //        {
                //            throw e;
                //        }
                //    }
                //    else
                //    {
                //        throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                //    }

                //} // End using context
            }
            else
            {
                throw new Exception("Invalid Ballot Accpted Code: " + ballotAcceptedCode.ToString());
            }

        } // End Reject Application

        private void CreateAuditLog()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Copy voted record to Audit table
                AvVotedRecordChangeLog audit = new AvVotedRecordChangeLog();

                audit.VoterId = Int32.Parse(Data.VoterID);
                audit.CountyCode = Data.County;
                audit.ElectionId = Data.ElectionID;
                audit.ApplicationIssued = Data.ApplicationIssued;
                audit.ApplicationAccepted = Data.ApplicationAccepted;
                audit.ApplicationRejected = Data.ApplicationRejected;
                audit.AppRejectedReason = Data.ApplicationRejectedReason;
                audit.DateIssued = Data.BallotIssued;
                audit.PrintedDate = Data.BallotPrinted;
                audit.AbsenteeId = Data.AbsenteeId;
                audit.AbsenteeVoterType = Data.AbsenteeType;
                audit.ApplicationSource = Data.ApplicationSource;
                audit.UocavaApplicationType = Data.UocavaApplicationType;
                audit.BallotDeliveryMethod = Data.BallotDeliveryMethod;
                audit.UocavaVoterEmail = Data.UocavaVoterEmail;
                audit.UocavaVoterFax = Data.UocavaVoterFax;
                audit.AddressType = Data.AddressType;
                audit.AddressLine1 = Data.DeliveryAddress1;
                audit.AddressLine2 = Data.DeliveryAddress2;
                audit.City = Data.DeliveryCity;
                audit.State = Data.DeliveryState;
                audit.Zip = Data.DeliveryZip;
                audit.Country = Data.DeliveryCountry;
                audit.PollId = Data.PollID;
                audit.DateVoted = Data.VotedDate;
                audit.PrecinctPartId = Data.PrecinctPartID;
                audit.BallotStyleId = Data.BallotStyleID;
                audit.Computer = Data.ComputerID;
                audit.BallotNumber = Data.BallotNumber;
                audit.SignRefused = Data.SignRefused??false;
                audit.LocalOnly = true;
                audit.UserId = Data.UserId;
                audit.LogCode = Data.LogCode;
                audit.ActivityCode = Data.ActivityCode;
                audit.ActivityDate = Data.ActivityDate;
                audit.ProvisionalOnly = Data.ProvisionalOnly;
                audit.NotTabulated = Data.NotTabulated;
                audit.BcMailDate = Data.BcMailDate;
                audit.LastSynced = DateTime.Parse("1/1/1");
                audit.CreatedOnDate = Data.CreatedOnDate.Value;
                audit.LastModified = DateTime.Now;

                audit.VotedRecordChangeId = Guid.NewGuid();
                audit.LogCodeNew = 6;
                audit.ChangeDate = DateTime.Now;
                audit.ChangeReason = "BALLOT REISSUED";

                context.AvVotedRecordChangeLogs.Add(audit);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private void CreateAuditLog(string reason)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Copy voted record to Audit table
                AvVotedRecordChangeLog audit = new AvVotedRecordChangeLog();

                audit.VoterId = Int32.Parse(Data.VoterID);
                audit.CountyCode = Data.County;
                audit.ElectionId = Data.ElectionID;
                audit.ApplicationIssued = Data.ApplicationIssued;
                audit.ApplicationAccepted = Data.ApplicationAccepted;
                audit.ApplicationRejected = Data.ApplicationRejected;
                audit.AppRejectedReason = Data.ApplicationRejectedReason;
                audit.DateIssued = Data.BallotIssued;
                audit.PrintedDate = Data.BallotPrinted;
                audit.AbsenteeId = Data.AbsenteeId;
                audit.AbsenteeVoterType = Data.AbsenteeType;
                audit.ApplicationSource = Data.ApplicationSource;
                audit.UocavaApplicationType = Data.UocavaApplicationType;
                audit.BallotDeliveryMethod = Data.BallotDeliveryMethod;
                audit.UocavaVoterEmail = Data.UocavaVoterEmail;
                audit.UocavaVoterFax = Data.UocavaVoterFax;
                audit.AddressType = Data.AddressType;
                audit.AddressLine1 = Data.DeliveryAddress1;
                audit.AddressLine2 = Data.DeliveryAddress2;
                audit.City = Data.DeliveryCity;
                audit.State = Data.DeliveryState;
                audit.Zip = Data.DeliveryZip;
                audit.Country = Data.DeliveryCountry;
                audit.PollId = Data.PollID;
                audit.DateVoted = Data.VotedDate;
                audit.PrecinctPartId = Data.PrecinctPartID;
                audit.BallotStyleId = Data.BallotStyleID;
                audit.Computer = Data.ComputerID;
                audit.BallotNumber = Data.BallotNumber;
                audit.SignRefused = Data.SignRefused??false;
                audit.LocalOnly = true;
                audit.UserId = Data.UserId;
                audit.LogCode = Data.LogCode;
                audit.ActivityCode = Data.ActivityCode;
                audit.ActivityDate = Data.ActivityDate;
                audit.ProvisionalOnly = Data.ProvisionalOnly;
                audit.NotTabulated = Data.NotTabulated;
                audit.BcMailDate = Data.BcMailDate;
                audit.LastSynced = DateTime.Parse("1/1/1");
                audit.CreatedOnDate = Data.CreatedOnDate.Value;
                audit.LastModified = DateTime.Now;

                audit.VotedRecordChangeId = Guid.NewGuid();
                audit.LogCodeNew = 6;
                audit.ChangeDate = DateTime.Now;
                audit.ChangeReason = reason;

                context.AvVotedRecordChangeLogs.Add(audit);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Update the existing record and return the status to ballot issued
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateReissued()
        {
            Data.BallotIssued = DateTime.Now;
            Data.BallotPrinted = DateTime.Now;
            Data.LogCode = LogCodeConstants.IssuedAbsenteeBallot;
            Data.ActivityCode = "A";
            Data.ActivityDate = DateTime.Now;
            Data.NotTabulated = false;

            Data.BallotRejectedDate = null;
            Data.BallotRejectedReason = null;

            if (Data.CreatedOnDate == DateTime.MinValue)
            {
                Data.CreatedOnDate = DateTime.Now;
            }
            else if (Data.CreatedOnDate == null)
            {
                Data.CreatedOnDate = DateTime.MinValue;
            }

            UpdateVoted("Reissued ballot");

            CreateAuditLog();

            //// Create disposable database context
            //using (var context = GetElectionContext())
            //{
            //    // Get existing Voted Record
            //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
            //    if (votedRecord != null)
            //    {
            //        votedRecord.LogCode = LogCodeConstants.IssuedAbsenteeBallot;
            //        votedRecord.PollId = Data.PollID;
            //        votedRecord.Computer = Data.ComputerID;
            //        votedRecord.DateIssued = DateTime.Now;
            //        votedRecord.PrintedDate = DateTime.Now;
            //        votedRecord.ActivityDate = DateTime.Now;
            //        //votedRecord.LocalOnly = true;

            //        // Update Mailing Address
            //        votedRecord.AddressLine1 = Data.DeliveryAddress1;
            //        votedRecord.AddressLine2 = Data.DeliveryAddress2;
            //        votedRecord.City = Data.DeliveryCity;
            //        votedRecord.State = Data.DeliveryState;
            //        votedRecord.Zip = Data.DeliveryZip;
            //        votedRecord.Country = Data.DeliveryCountry;
            //        votedRecord.TempAddress = Data.TempAddress.Value;

            //        // Intelligent Barcodes
            //        votedRecord.IMBOut = Data.OutGoingIMB;
            //        votedRecord.IMBIn = Data.InComingIMB;

            //        // Clear rejected values
            //        votedRecord.BallotRejectedDate = null;
            //        votedRecord.BallotRejectedReason = null;

            //        // PREPARE UPLOAD
            //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

            //        // CREATE VOTED HISTORY LOG
            //        CreateHistory(context, votedRecord, "Reissued ballot");

            //        // Copy voted record to Audit table
            //        AvVotedRecordChangeLog audit = new AvVotedRecordChangeLog();

            //        audit.VoterId = votedRecord.VoterId;
            //        audit.CountyCode = votedRecord.CountyCode;
            //        audit.ElectionId = votedRecord.ElectionId;
            //        audit.ApplicationIssued = votedRecord.ApplicationIssued;
            //        audit.ApplicationAccepted = votedRecord.ApplicationAccepted;
            //        audit.ApplicationRejected = votedRecord.ApplicationRejected;
            //        audit.AppRejectedReason = votedRecord.AppRejectedReason;
            //        audit.DateIssued = votedRecord.DateIssued;
            //        audit.PrintedDate = votedRecord.PrintedDate;
            //        audit.AbsenteeId = votedRecord.AbsenteeId;
            //        audit.AbsenteeVoterType = votedRecord.AbsenteeVoterType;
            //        audit.ApplicationSource = votedRecord.ApplicationSource;
            //        audit.UocavaApplicationType = votedRecord.UocavaApplicationType;
            //        audit.BallotDeliveryMethod = votedRecord.BallotDeliveryMethod;
            //        audit.UocavaVoterEmail = votedRecord.UocavaVoterEmail;
            //        audit.UocavaVoterFax = votedRecord.UocavaVoterFax;
            //        audit.AddressType = votedRecord.AddressType;
            //        audit.AddressLine1 = votedRecord.AddressLine1;
            //        audit.AddressLine2 = votedRecord.AddressLine2;
            //        audit.City = votedRecord.City;
            //        audit.State = votedRecord.State;
            //        audit.Zip = votedRecord.Zip;
            //        audit.Country = votedRecord.Country;
            //        audit.PollId = votedRecord.PollId;
            //        audit.DateVoted = votedRecord.DateVoted;
            //        audit.PrecinctPartId = votedRecord.PrecinctPartId;
            //        audit.BallotStyleId = votedRecord.BallotStyleId;
            //        audit.Computer = votedRecord.Computer;
            //        audit.BallotNumber = votedRecord.BallotNumber;
            //        audit.SignRefused = votedRecord.SignRefused;
            //        audit.LocalOnly = true;
            //        audit.UserId = votedRecord.UserId;
            //        audit.LogCode = votedRecord.LogCode;
            //        audit.ActivityCode = votedRecord.ActivityCode;
            //        audit.ActivityDate = votedRecord.ActivityDate;
            //        audit.ProvisionalOnly = votedRecord.ProvisionalOnly;
            //        audit.NotTabulated = votedRecord.NotTabulated;
            //        audit.BcMailDate = votedRecord.BcMailDate;
            //        audit.LastSynced = DateTime.Parse("1/1/1");
            //        audit.CreatedOnDate = votedRecord.CreatedOnDate;
            //        audit.LastModified = DateTime.Now;

            //        audit.VotedRecordChangeId = Guid.NewGuid();
            //        audit.LogCodeNew = 6;
            //        audit.ChangeDate = DateTime.Now;
            //        audit.ChangeReason = "BALLOT REISSUED";

            //        context.AvVotedRecordChangeLogs.Add(audit);

            //        try
            //        {
            //            context.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            throw e;
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
            //    }

            //} // End using context

        } // End Update Reissued

        /// <summary>
        /// Update the existing record and set the poll id, machine number and user id
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateLocation()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.PollId = Data.PollID;
                    votedRecord.Computer = Data.ComputerID;
                    votedRecord.UserId = Data.ComputerID;

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Prepared Batch");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Location

        /// <summary>
        /// Update the existing record and set the poll id, machine number and user id
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateUser(int? userId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.UserId = userId;

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Prepared Batch");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Location

        /// <summary>
        /// Update the existing record and set the phone and e-mail
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public void UpdateContact()
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    //votedRecord.PollId = Data.PollID;
                    //votedRecord.Computer = Data.ComputerID;
                    //votedRecord.UserId = Data.ComputerID;
                    votedRecord.VoterContactEmail = Data.Email;
                    votedRecord.VoterContactPhone = Data.Phone;

                    // CREATE VOTED HISTORY LOG
                    //CreateHistory(context, votedRecord, "Changed Contact Details");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());
                }

            } // End using context

        } // End Update Location

        public void UpdateForBatch(int? userId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.UserId = userId;

                    // Intelligent Barcodes
                    votedRecord.IMBOut = Data.OutGoingIMB;
                    votedRecord.IMBIn = Data.InComingIMB;

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Prepared Batch");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    //throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());                    
                }

            } // End using context

        } // End Update Location

        public void UpdateForBatch(Guid? batchId)
        {
            // Create disposable database context
            using (var context = GetElectionContext())
            {
                // Get existing Voted Record
                var votedRecord = context.AvActivities.Find(Int32.Parse(Data.VoterID));
                if (votedRecord != null)
                {
                    votedRecord.BatchId = batchId;

                    // Intelligent Barcodes
                    votedRecord.IMBOut = Data.OutGoingIMB;
                    votedRecord.IMBIn = Data.InComingIMB;

                    // CREATE VOTED HISTORY LOG
                    CreateHistory(context, votedRecord, "Prepared Batch");

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    //throw new Exception("Voter not found for ID: " + Data.VoterID.ToString());

                    Data.BatchID = batchId;

                    // Create new record
                    UpdateVoted("Prepared Batch");
                }

            } // End using context

        } // End Update Location

        public void ChangeLogCode(DateTime receivedDate, int newLogCode, string auditReason, int? rejectedReasonId)
        {
            var acceptableCodes = new List<int> { 9, 10, 12, 14 };
            if (acceptableCodes.Contains(newLogCode))
            {
                // Copy voted record to Audit table before changes are made
                CreateAuditLog(auditReason);

                // Set voted date
                LogCodes ballotLogCode = (LogCodes)Enum.ToObject(typeof(LogCodes), newLogCode);
                if (ballotLogCode == LogCodes.ReturnedAbsenteeBallot ||
                        ballotLogCode == LogCodes.UnsignedAbsenteeBallot)
                {
                    Data.VotedDate = receivedDate;
                }
                else
                {
                    Data.VotedDate = null;
                }

                int? oldLogcode = Data.LogCode;
                Data.LogCode = newLogCode;

                // Set activity code
                if ((int)newLogCode == 9)
                {
                    Data.ActivityCode = "A";

                    // Clear rejected values
                    Data.BallotRejectedDate = null;
                    Data.BallotRejectedReason = null;
                }
                else
                {
                    Data.ActivityCode = null;
                }

                Data.ActivityDate = receivedDate;
                Data.NotTabulated = false;

                Data.BallotRejectedDate = null;
                Data.BallotRejectedReason = null;

                UpdateVoted("Manual log code change from " + oldLogcode + " to " + newLogCode);                

                //// Create disposable database context
                //using (var context = GetElectionContext())
                //{
                //    // Get existing Voted Record
                //    var votedRecord = context.AvVotedRecords.Find(Int32.Parse(Data.VoterID));
                //    if (votedRecord != null)
                //    {
                //        // Copy voted record to Audit table
                //        AvVotedRecordChangeLog audit = new AvVotedRecordChangeLog();

                //        audit.VoterId = votedRecord.VoterId;
                //        audit.CountyCode = votedRecord.CountyCode;
                //        audit.ElectionId = votedRecord.ElectionId;
                //        audit.ApplicationIssued = votedRecord.ApplicationIssued;
                //        audit.ApplicationAccepted = votedRecord.ApplicationAccepted;
                //        audit.ApplicationRejected = votedRecord.ApplicationRejected;
                //        audit.AppRejectedReason = votedRecord.AppRejectedReason;
                //        audit.DateIssued = votedRecord.DateIssued;
                //        audit.PrintedDate = votedRecord.PrintedDate;
                //        audit.AbsenteeId = votedRecord.AbsenteeId;
                //        audit.AbsenteeVoterType = votedRecord.AbsenteeVoterType;
                //        audit.ApplicationSource = votedRecord.ApplicationSource;
                //        audit.UocavaApplicationType = votedRecord.UocavaApplicationType;
                //        audit.BallotDeliveryMethod = votedRecord.BallotDeliveryMethod;
                //        audit.UocavaVoterEmail = votedRecord.UocavaVoterEmail;
                //        audit.UocavaVoterFax = votedRecord.UocavaVoterFax;
                //        audit.AddressType = votedRecord.AddressType;
                //        audit.AddressLine1 = votedRecord.AddressLine1;
                //        audit.AddressLine2 = votedRecord.AddressLine2;
                //        audit.City = votedRecord.City;
                //        audit.State = votedRecord.State;
                //        audit.Zip = votedRecord.Zip;
                //        audit.Country = votedRecord.Country;
                //        audit.PollId = votedRecord.PollId;
                //        audit.DateVoted = votedRecord.DateVoted;
                //        audit.PrecinctPartId = votedRecord.PrecinctPartId;
                //        audit.BallotStyleId = votedRecord.BallotStyleId;
                //        audit.Computer = votedRecord.Computer;
                //        audit.BallotNumber = votedRecord.BallotNumber;
                //        audit.SignRefused = votedRecord.SignRefused;
                //        audit.LocalOnly = true;
                //        audit.UserId = votedRecord.UserId;
                //        audit.LogCode = votedRecord.LogCode;
                //        audit.ActivityCode = votedRecord.ActivityCode;
                //        audit.ActivityDate = votedRecord.ActivityDate;
                //        audit.ProvisionalOnly = votedRecord.ProvisionalOnly;
                //        //audit.FledVoter = votedRecord.FledVoter;
                //        //audit.WrongVoter = votedRecord.WrongVoter;
                //        audit.NotTabulated = votedRecord.NotTabulated;
                //        audit.BcMailDate = votedRecord.BcMailDate;
                //        audit.LastSynced = DateTime.Parse("1/1/1");
                //        audit.CreatedOnDate = votedRecord.CreatedOnDate;
                //        audit.LastModified = DateTime.Now;

                //        audit.VotedRecordChangeId = Guid.NewGuid();
                //        audit.LogCodeNew = newLogCode;
                //        audit.ChangeDate = receivedDate;
                //        audit.ChangeReason = auditReason;

                //        context.AvVotedRecordChangeLogs.Add(audit);

                //        // Update avVotedRecord
                //        LogCodes ballotLogCode = (LogCodes)Enum.ToObject(typeof(LogCodes), newLogCode);
                //        if (ballotLogCode == LogCodes.ReturnedAbsenteeBallot ||
                //                ballotLogCode == LogCodes.UnsignedAbsenteeBallot)
                //        {
                //            votedRecord.DateVoted = receivedDate;
                //        }
                //        else
                //        {
                //            votedRecord.DateVoted = null;
                //        }
                //        votedRecord.BallotStyleId = votedRecord.BallotStyleId;
                //        votedRecord.PrecinctPartId = votedRecord.PrecinctPartId;
                //        votedRecord.PollId = Data.PollID;
                //        votedRecord.Computer = Data.ComputerID;
                //        votedRecord.UserId = Data.UserId;
                //        votedRecord.LogCode = newLogCode;
                //        if ((int)newLogCode == 9)
                //        {
                //            votedRecord.ActivityCode = "A";

                //            // Clear rejected values
                //            votedRecord.BallotRejectedDate = null;
                //            votedRecord.BallotRejectedReason = null;
                //        }
                //        else
                //        {
                //            votedRecord.ActivityCode = null;
                //        }
                //        if(rejectedReasonId != null)
                //        {
                //            //votedRecord.RejectedReasonId = rejectedReasonId;
                //            votedRecord.BallotRejectedDate = Data.BallotRejectedDate;
                //            votedRecord.BallotRejectedReason = Data.BallotRejectedReason;
                //        }
                //        votedRecord.ActivityDate = receivedDate;
                //        //votedRecord.LocalOnly = true;
                //        votedRecord.LastSynced = DateTime.Parse("1/1/1");
                //        //votedRecord.ForcePropagate = true;

                //        // PREPARE UPLOAD
                //        AddToUploadQueue(context, Int32.Parse(Data.VoterID), Data.PollID);

                //        // CREATE VOTED HISTORY LOG
                //        CreateHistory(context, votedRecord, "Manual log code change from " + audit.LogCode + " to " + audit.LogCodeNew);

                //        try
                //        {
                //            context.SaveChanges();
                //        }
                //        catch (Exception e)
                //        {
                //            throw e; 
                //        }
                //    }
                //}// End using context
            }
            else
            {
                throw new Exception("Invalid Ballot Accepted Code: " + 
                    ((LogCodes)Enum.ToObject(typeof(LogCodes), newLogCode)).ToString());
            }

        } // End Change Log Code

        #endregion

        #region ValidationMethods
        // This function checks if the voter has already voted and where
        // for deciding what options are availible to the user 
        public VoterLookupStatus CheckStatus(int pollId)
        {
            VoterLookupStatus voterStatus = VoterLookupStatus.None;

            // Check deleted status
            if (Data.WasRemoved())
            {
                voterStatus = VoterLookupStatus.Deleted;
            }
            // Check provisional Only
            else if (Data.ProvisionalOnly == true)
            {
                voterStatus = VoterLookupStatus.Provisional;
            }
            // Check log code
            else if (Data.HasVoted())
            {
                //Check voter location and logdate
                if (Data.VotedHereToday(pollId) && !Data.WrongOrFledVoter())
                {
                    // if same location then spoil ballot
                    voterStatus = VoterLookupStatus.Spoilable;
                }
                else
                {
                    // if different location or date then provisional ballot
                    voterStatus = VoterLookupStatus.Provisional;
                }
            }
            // Check if voter has a valid ballot style
            else if (!Data.IsEligible())
            {
                //IneligibleVoter
                voterStatus = VoterLookupStatus.Ineligible;
            }

            // Add precinct check for Hybrid sites (set voterStatus = VoterLookupStatus.Provisional when not for current site)
            // Voter gets official ballot if they exist in ElectionPrecinctPoll
            // Other wise they get a provisional

            else
            {
                // Allow user to procceed to print out a ballot
                voterStatus = VoterLookupStatus.Eligible;
            }

            return voterStatus;
        }

        public VoterLookupStatus CheckStatusHybrid(int pollId)
        {
            VoterLookupStatus voterStatus = VoterLookupStatus.None;

            // Load the valid hybrid poll id
            Data.ValidPollID = ValidatePrecinctPoll(pollId);

            // Check deleted status
            if (Data.WasRemoved())
            {
                voterStatus = VoterLookupStatus.Deleted;
            }
            // Check provisional Only
            else if (Data.ProvisionalOnly == true)
            {
                voterStatus = VoterLookupStatus.Provisional;
            }
            // Check log code
            else if (Data.HasVoted())
            {

                //Check voter location and logdate
                if (Data.VotedHereToday(pollId) && !Data.WrongOrFledVoter())
                {
                    // if same location then spoil ballot
                    voterStatus = VoterLookupStatus.Spoilable;
                }
                else
                {
                    // if different location or date then provisional ballot
                    voterStatus = VoterLookupStatus.Provisional;
                }
            }
            else if (Data.ValidPollID == null)
            {
                voterStatus = VoterLookupStatus.Hybrid;
            }
            // Check if voter has a valid ballot style
            else if (!Data.IsEligible())
            {
                //IneligibleVoter
                voterStatus = VoterLookupStatus.Ineligible;
            }

            // Add precinct check for Hybrid sites (set voterStatus = VoterLookupStatus.Provisional when not for current site)
            // Voter gets official ballot if they exist in ElectionPrecinctPoll
            // Other wise they get a provisional

            else
            {
                // Allow user to procceed to print out a ballot
                voterStatus = VoterLookupStatus.Eligible;
            }

            return voterStatus;
        }
        #endregion
    }
}
