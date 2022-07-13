using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using VoterX.Core.Context;
using System.Collections.ObjectModel;
using VoterX.Core.Voters;
using System.Data.SqlClient;
using System.Data.Entity;

namespace VoterX.Core.Voters
{
    public class VoterFactory : IDisposable
    {
        private bool disposed;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private int? _electionType;
        private string _connection;
        public string Connection
        {
            get
            {
                return _connection;
            }
        }

        public VoterFactory() : this(null) { }
        public VoterFactory(int? type)
        {
            _electionType = type;
        }
        public VoterFactory(int? type, string connection)
        {
            _electionType = type;
            _connection = connection;
        }

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
            using (var context = GetElectionContext())
            {
                try
                {
                    context.Database.Connection.Open();
                    context.Database.Connection.Close();
                }
                catch (SqlException)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<bool> ExistsAsync()
        {
            return await Task.Run(() =>
            {
                using (var context = GetElectionContext())
                {
                    try
                    {
                        context.Database.Connection.Open();
                        context.Database.Connection.Close();
                    }
                    catch (SqlException)
                    {
                        return false;
                    }
                    return true;
                }
            });
        }

        private ElectionContext GetElectionContext()
        {
            if (_connection != null)
                return new ElectionContext(_connection);
            else
                return new ElectionContext();
        }

        internal List<NMVoter> ConvertLists(List<VoterDataModel> voterList)
        {
            List<NMVoter> newList = new List<NMVoter>();
            foreach (var voter in voterList)
            {
                NMVoter newVoter = new NMVoter(voter);
                newVoter.ConnectionString = _connection;
                newList.Add(newVoter);
            }
            return newList;
        }

        // https://www.c-sharpcorner.com/UploadFile/ff2f08/entity-framework-and-asnotracking/

        internal IQueryable<VoterDataModel> QueryTest(ElectionContext context)
        {
            var query = from voters in context.SosVoters
                        select new
                        {
                            voters.VoterId,
                            voters.LastName,
                            voters.FirstName,
                            voters.MiddleName,
                            voters.Title,
                            voters.Suffix,
                            voters.DateOfBirth,
                            voters.Gender,
                            voters.VoterStatus,
                            voters.Sid,
                            voters.HouseNumber,
                            voters.StreetPrefix,
                            voters.StreetName,
                            voters.StreetType,
                            voters.StreetSuffix,
                            voters.Unit,
                            voters.UnitNumber,
                            voters.NonStdAddress,
                            voters.NonStdAddressDescription,
                            voters.City,
                            voters.State,
                            voters.Zip,
                            voters.MailingAddress,
                            voters.MailingAddress2,
                            voters.MailingCity,
                            voters.MailingState,
                            voters.MailingZip,
                            voters.MailingCountry,
                            voters.CountyCode,
                            voters.PrecinctPartId,
                            voters.Party,
                            voters.Dl,
                            voters.IdRequired
                        };

            return query
                .Select(v => new VoterDataModel
                {
                    VoterID = v.VoterId.ToString(),
                    LastName = v.LastName,
                    FirstName = v.FirstName,
                    MiddleName = v.MiddleName,
                    Generation = v.Suffix,
                    Title = v.Title,
                    FullName = String.Concat(
                        v.Title, " ",
                        v.FirstName, " ",
                        v.MiddleName, " ",
                        v.LastName, " ",
                        v.Suffix)
                        .Trim().Replace("  ", " "),
                    DOBYear = v.DateOfBirth,
                    Gender = v.Gender,
                    Party = v.Party,
                    DriversLicense = v.Dl,
                    IDRequired = (bool)v.IdRequired,
                    Status = v.VoterStatus,
                    SID = v.Sid,
                    HouseNumber = v.HouseNumber,
                    StreetPrefix = v.StreetPrefix,
                    StreetName = v.StreetName,
                    StreetType = v.StreetType,
                    StreetSuffix = v.StreetSuffix,
                    Unit = v.Unit,
                    UnitNumber = v.UnitNumber,
                    Address1 = String.Concat(
                        v.HouseNumber, " ",
                        v.StreetPrefix, " ",
                        v.StreetName, " ",
                        v.StreetType, " ",
                        v.StreetSuffix)
                        .Trim()
                        .Replace("  ", " "),
                    Address2 = String.Concat(
                        v.Unit, " ",
                        v.UnitNumber)
                        .Trim()
                        .Replace("  ", " "),
                    City = v.City,
                    State = v.State,
                    Zip = v.Zip,
                    NonStandardAddressFlag = (bool)v.NonStdAddress,
                    NonStandardAddressDescription = v.NonStdAddressDescription,
                    MailingAddress1 = v.MailingAddress,
                    MailingAddress2 = v.MailingAddress2,
                    MailingCity = v.MailingCity,
                    MailingState = v.MailingState,
                    MailingZip = v.MailingZip,
                    PrecinctPartID = v.PrecinctPartId ?? ""
                }
                );
        }

        internal IQueryable<VoterDataModel> Query(ElectionContext context)
        {
            //var votedQuery = from voters in JoinBallotStyle(context, _electionType)
            //                 join votedrecord in context.AvVotedRecords
            //                     on voters.VoterId equals votedrecord.VoterId into votedrecordgroup
            //                 from votedRecord in votedrecordgroup.DefaultIfEmpty()
            //                 join votedActivity in context.AvActivities
            //                     on voters.VoterId equals votedActivity.VoterId into votedactivitygroup
            //                 from activity in votedactivitygroup.DefaultIfEmpty()
            //                 select new
            //                 {
            //                     voters.VoterId,
            //                     voters.LastName,
            //                     voters.FirstName,
            //                     voters.MiddleName,
            //                     voters.Title,
            //                     voters.Suffix,
            //                     voters.DateOfBirth,
            //                     voters.Gender,
            //                     voters.VoterStatus,
            //                     voters.Sid,
            //                     voters.HouseNumber,
            //                     voters.StreetPrefix,
            //                     voters.StreetName,
            //                     voters.StreetType,
            //                     voters.StreetSuffix,
            //                     voters.Unit,
            //                     voters.UnitNumber,
            //                     voters.NonStdAddress,
            //                     voters.NonStdAddressDescription,
            //                     voters.City,
            //                     voters.State,
            //                     voters.Zip,
            //                     voters.MailingAddress,
            //                     voters.MailingAddress2,
            //                     voters.MailingCity,
            //                     voters.MailingState,
            //                     voters.MailingZip,
            //                     voters.MailingCountry,
            //                     voters.CountyCode,
            //                     voters.PrecinctPartId,
            //                     voters.Party,
            //                     voters.Dl,
            //                     voters.SSN,
            //                     voters.IdRequired,
            //                     voters.BallotStyleId,
            //                     voters.BallotStyleName,
            //                     voters.BallotStyleFileName,
            //                     voters.ModificationType,
            //                     voters.LastModified,

            //                     //voted = votedRecord.ActivityDate > activity.ActivityDate || activity.ActivityDate == null ? votedRecord : activity

            //                     LogCode = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.LogCode : activity.LogCode,
            //                     PollId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.PollId : activity.PollId,
            //                     ApplicationIssued = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationIssued : activity.ApplicationIssued,
            //                     ApplicationAccepted = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationAccepted : activity.ApplicationAccepted,
            //                     ApplicationRejected = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationRejected : activity.ApplicationRejected,
            //                     DateIssued = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.DateIssued : activity.DateIssued,
            //                     PrintedDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.PrintedDate : activity.PrintedDate,
            //                     AbsenteeId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AbsenteeId : activity.AbsenteeId,
            //                     AbsenteeVoterType = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AbsenteeVoterType : activity.AbsenteeVoterType,
            //                     ApplicationSource = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationSource : activity.ApplicationSource,
            //                     UocavaApplicationType = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UocavaApplicationType : activity.UocavaApplicationType,
            //                     UocavaVoterEmail = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UocavaVoterEmail : activity.UocavaVoterEmail,
            //                     UocavaVoterFax = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UocavaVoterFax : activity.UocavaVoterFax,
            //                     BallotDeliveryMethod = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotDeliveryMethod : activity.BallotDeliveryMethod,
            //                     AddressType = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AddressType : activity.AddressType,
            //                     DeliveryAddress1 = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AddressLine1 : activity.AddressLine1,
            //                     DeliveryAddress2 = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AddressLine2 : activity.AddressLine2,
            //                     DeliveryState = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.State : activity.State,
            //                     DeliveryCity = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.City : activity.City,
            //                     DeliveryZip = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.Zip : activity.Zip,
            //                     DeliveryCountry = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.Country : activity.Country,
            //                     TempAddress = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.TempAddress : activity.TempAddress,
            //                     DateVoted = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.DateVoted : activity.DateVoted,
            //                     Computer = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.Computer : activity.Computer,
            //                     BallotNumber = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotNumber : activity.BallotNumber,
            //                     SignRefused = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.SignRefused : activity.SignRefused,
            //                     UserId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UserId : activity.UserId,
            //                     ActivityCode = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ActivityCode : activity.ActivityCode,
            //                     ActivityDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ActivityDate : activity.ActivityDate,
            //                     ProvisionalOnly = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ProvisionalOnly : activity.ProvisionalOnly,
            //                     NotTabulated = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.NotTabulated : activity.NotTabulated,
            //                     AppRejectedReason = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AppRejectedReason : activity.AppRejectedReason,
            //                     SpoiledReason = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.SpoiledReason : activity.SpoiledReason,
            //                     BcMailDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BcMailDate : activity.BcMailDate,
            //                     IMBOut = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.IMBOut : activity.IMBOut,
            //                     IMBIn = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.IMBIn : activity.IMBIn,
            //                     VoterContactEmail = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.VoterContactEmail : activity.VoterContactEmail,
            //                     VoterContactPhone = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.VoterContactPhone : activity.VoterContactPhone,
            //                     BallotRejectedDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotRejectedDate : activity.BallotRejectedDate,
            //                     BallotRejectedReason = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotRejectedReason : activity.BallotRejectedReason,
            //                     ServisABSLastModified = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ServisABSLastModified : activity.ServisABSLastModified,
            //                     ServisVotedLastModified = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ServisABSLastModified : activity.ServisABSLastModified,
            //                     BatchId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BatchId : activity.BatchId,
            //                     IgnoreRules = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ForcePropagate : activity.IgnoreRules,
            //                     CreatedOnDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.CreatedOnDate : activity.CreatedOnDate
            //                 };

            var query = from voters in JoinBallotStyle(context, _electionType)
                        join votedrecord in JoinVotedRecords(context)
                            on voters.VoterId equals votedrecord.VoterId into votedrecordgroup
                        from voted in votedrecordgroup.DefaultIfEmpty()
                        join serverIdRequired in context.AvVotedRecords
                            on voters.VoterId equals serverIdRequired.VoterId into serverIdRequired
                        from idRequired in serverIdRequired.DefaultIfEmpty()
                            //join votedActivity in context.AvActivities
                            //    on voters.VoterId equals votedActivity.VoterId into votedactivitygroup
                            //from activity in votedactivitygroup.DefaultIfEmpty()
                        join votedlogcode in context.AvLogCodes
                            on voted.LogCode equals votedlogcode.LogCode into votedlogcodegroup
                        from logcode in votedlogcodegroup.DefaultIfEmpty()
                        join votedpollsite in context.SosLocations
                            on voted.PollId equals votedpollsite.PollId into votedpollsitegroup
                        from pollsite in votedpollsitegroup.DefaultIfEmpty()
                        select new
                        {
                            voters.VoterId,
                            voters.LastName,
                            voters.FirstName,
                            voters.MiddleName,
                            voters.Title,
                            voters.Suffix,
                            voters.DateOfBirth,
                            voters.Gender,
                            voters.VoterStatus,
                            voters.Sid,
                            voters.HouseNumber,
                            voters.StreetPrefix,
                            voters.StreetName,
                            voters.StreetType,
                            voters.StreetSuffix,
                            voters.Unit,
                            voters.UnitNumber,
                            voters.NonStdAddress,
                            voters.NonStdAddressDescription,
                            voters.City,
                            voters.State,
                            voters.Zip,
                            voters.MailingAddress,
                            voters.MailingAddress2,
                            voters.MailingCity,
                            voters.MailingState,
                            voters.MailingZip,
                            voters.MailingCountry,
                            voters.CountyCode,
                            voters.PrecinctPartId,
                            //voterprecinct.PrecinctPart,
                            //voterprecinct.Precinct,
                            voters.Party,
                            voters.Dl,
                            voters.SSN,
                            VotersIdRequired = voters.IdRequired,
                            voted.LogCode,
                            logcode.LogDescription,
                            voted.DateVoted,
                            voted.ActivityDate,
                            voted.ActivityCode,
                            voted.PollId,
                            //voted.FledVoter,
                            //voted.WrongVoter,
                            voted.NotTabulated,
                            voted.BallotNumber,
                            voted.SignRefused,
                            pollsite.PlaceName,
                            //ballotstyle.BallotStyles.BallotStyleId,
                            //ballotstyle.BallotStyles.BallotStyleName,
                            //ballotstyle.BallotStyles.BallotStyleFileName,
                            voters.BallotStyleId,
                            voters.BallotStyleName,
                            voters.BallotStyleFileName,
                            voters.ModificationType,
                            voters.LastModified,
                            voted.PrintedDate,
                            voted.Computer,
                            voted.UserId,
                            voted.AbsenteeVoterType,
                            voted.ApplicationIssued,
                            voted.ApplicationAccepted,
                            voted.ApplicationRejected,
                            voted.ApplicationSource,
                            voted.AppRejectedReason,
                            voted.IMBOut,
                            voted.IMBIn,
                            voted.TempAddress,
                            voted.AddressType,
                            voted.BallotRejectedDate,
                            voted.BallotRejectedReason,
                            DeliveryAddress1 = voted.AddressLine1,
                            DeliveryAddress2 = voted.AddressLine2,
                            DeliveryState = voted.State,
                            DeliveryCity = voted.City,
                            DeliveryZip = voted.Zip,
                            DeliveryCountry = voted.Country,
                            voted.VoterContactEmail,
                            voted.VoterContactPhone,
                            voted.DateIssued,
                            voted.AbsenteeId,
                            voted.UocavaApplicationType,
                            voted.UocavaVoterEmail,
                            voted.UocavaVoterFax,
                            voted.BallotDeliveryMethod,
                            voted.ProvisionalOnly,
                            voted.SpoiledReason,
                            voted.BcMailDate,
                            voted.ServisABSLastModified,
                            voted.ServisVotedLastModified,
                            voted.BatchId,
                            IgnoreRules = voted.ForcePropagate,
                            VotedIdRequired = idRequired.IdRequired,
                            voted.CreatedOnDate                            
                        };

            return query
                .Select(v => new VoterDataModel
                {
                    VoterID = v.VoterId.ToString(),
                    LastName = v.LastName,
                    FirstName = v.FirstName,
                    MiddleName = v.MiddleName,
                    Generation = v.Suffix,
                    Title = v.Title,
                    FullName = String.Concat(
                        v.Title, " ",
                        v.FirstName, " ",
                        v.MiddleName, " ",
                        v.LastName, " ",
                        v.Suffix)
                        .Trim().Replace("  ", " "),
                    DOBYear = v.DateOfBirth,
                    DOBSearch = v.DateOfBirth,
                    Gender = v.Gender,
                    Party = v.Party,
                    DriversLicense = v.Dl,
                    IDRequired = v.VotersIdRequired,
                    VotedIdRequired = v.VotedIdRequired,
                    //IDRequired = false,
                    Status = v.VoterStatus,
                    DL = v.Dl,
                    SSN = v.SSN,
                    SID = v.Sid,
                    HouseNumber = v.HouseNumber,
                    StreetPrefix = v.StreetPrefix,
                    StreetName = v.StreetName,
                    StreetType = v.StreetType,
                    StreetSuffix = v.StreetSuffix,
                    Unit = v.Unit,
                    UnitNumber = v.UnitNumber,
                    Address1 = v.NonStdAddress != null && v.NonStdAddress == true ? v.NonStdAddressDescription :
                        String.Concat(
                        v.HouseNumber, " ",
                        v.StreetPrefix, " ",
                        v.StreetName, " ",
                        v.StreetType, " ",
                        v.StreetSuffix)
                        .Trim()
                        .Replace("  ", " "),
                    Address2 = String.Concat(
                        v.Unit, " ",
                        v.UnitNumber)
                        .Trim()
                        .Replace("  ", " "),
                    City = v.City,
                    State = v.State,
                    Zip = v.Zip,
                    NonStandardAddressFlag = (bool)v.NonStdAddress.Value,
                    NonStandardAddressDescription = v.NonStdAddressDescription,
                    MailingAddress1 = v.MailingAddress,
                    MailingAddress2 = v.MailingAddress2,
                    MailingCity = v.MailingCity,
                    MailingState = v.MailingState,
                    MailingZip = v.MailingZip,
                    MailingCSZ = String.Concat(
                        v.MailingCity, ", ",
                        v.State, " ",
                        v.MailingZip)
                        .Trim()
                        .Replace("  ", " "),
                    MailingCountry = v.MailingCountry,
                    County = v.CountyCode,
                    DeliveryAddress1 = v.DeliveryAddress1,
                    DeliveryAddress2 = v.DeliveryAddress2,
                    DeliveryState = v.DeliveryState,
                    DeliveryCity = v.DeliveryCity,
                    DeliveryZip = v.DeliveryZip,
                    DeliveryCountry = v.DeliveryCountry,
                    //TempAddress = v.TempAddress,
                    TempAddress = false,
                    AddressType = v.AddressType,
                    PrecinctPartID = v.PrecinctPartId ?? "",

                    // LOG CODE CONDITIONS
                    // If the Voters Modified Type is D or precinct part is missing then use Log Code 17
                    //LogCode = v.LogCode,
                    LogCode = v.ModificationType == "D" || v.PrecinctPartId == null ? 17 : v.LogCode == null ? 1 : v.LogCode,

                    // LOG DESCRIPTION CONDITIONS
                    // If precinct part is missing display "No Precinct"
                    LogDescription = v.PrecinctPartId == null ? "NO PRECINCT" :
                    // or if Modified Type is D then display "Voter Removed"
                    v.ModificationType == "D" ? "VOTER REMOVED" :
                    // or if there is no log code then display "Registered to Vote"
                    v.LogDescription == null ? "REGISTERED TO VOTE" :
                    v.LogDescription.ToUpper(),

                    BallotStyleID = v.BallotStyleId,
                    BallotStyle = v.BallotStyleName,
                    BallotStyleFile = v.BallotStyleFileName,

                    PollID = v.PollId,
                    PollName = v.PlaceName,
                    ApplicationIssued = v.ApplicationIssued,
                    ApplicationAccepted = v.ApplicationAccepted,
                    ApplicationRejected = v.ApplicationRejected,
                    BallotIssued = v.DateIssued,
                    BallotPrinted = v.PrintedDate,
                    AbsenteeId = v.AbsenteeId,
                    AbsenteeType = v.AbsenteeVoterType,
                    ApplicationSource = v.ApplicationSource,
                    UocavaApplicationType = v.UocavaApplicationType,
                    UocavaVoterEmail = v.UocavaVoterEmail,
                    UocavaVoterFax = v.UocavaVoterFax,
                    BallotDeliveryMethod = v.BallotDeliveryMethod,
                    VotedDate = v.DateVoted,
                    ComputerID = v.Computer,
                    BallotNumber = v.BallotNumber,
                    //SignRefused = (bool)v.SignRefused,
                    SignRefused = false,
                    UserId = v.UserId,
                    ActivityDate = v.ActivityDate,
                    ActivityCode = v.ActivityCode,
                    //ProvisionalOnly = v.ProvisionalOnly,
                    ProvisionalOnly = false,
                    //NotTabulated = v.NotTabulated,
                    NotTabulated = false,
                    ApplicationRejectedReason = v.AppRejectedReason,
                    SpoiledReasonID = v.SpoiledReason,
                    BcMailDate = v.BcMailDate,
                    OutGoingIMB = v.IMBOut,
                    InComingIMB = v.IMBIn,
                    Phone = v.VoterContactPhone,
                    Email = v.VoterContactEmail,
                    BallotRejectedDate = v.BallotRejectedDate,
                    BallotRejectedReason = v.BallotRejectedReason,
                    ServisABSLastModified = v.ServisABSLastModified,
                    ServisVotedLastModified = v.ServisVotedLastModified,
                    BatchID = v.BatchId,
                    CreatedOnDate = v.CreatedOnDate,

                    PartyVisibility = false,
                    OutofCountry = false,
                    BallotSurrendered = false,
                    FledVoter = false,
                    WrongVoter = false
                }
                );
        }

        internal IQueryable<VoterDataModel> QuerySimple(ElectionContext context)
        {
            var query = from voters in JoinBallotStyle(context, _electionType)
                        join votedrecord in JoinVotedRecordsSimple(context)
                            on voters.VoterId equals votedrecord.VoterId into votedrecordgroup
                        from voted in votedrecordgroup.DefaultIfEmpty()
                        join serverIdRequired in context.AvVotedRecords
                            on voters.VoterId equals serverIdRequired.VoterId into serverIdRequired
                        from idRequired in serverIdRequired.DefaultIfEmpty()
                        join votedlogcode in context.AvLogCodes
                            on voted.LogCode equals votedlogcode.LogCode into votedlogcodegroup
                        from logcode in votedlogcodegroup.DefaultIfEmpty()
                        join votedpollsite in context.SosLocations
                            on voted.PollId equals votedpollsite.PollId into votedpollsitegroup
                        from pollsite in votedpollsitegroup.DefaultIfEmpty()
                        select new
                        {
                            voters.VoterId,
                            voters.LastName,
                            voters.FirstName,
                            voters.MiddleName,
                            voters.Title,
                            voters.Suffix,
                            voters.DateOfBirth,
                            voters.Gender,
                            voters.VoterStatus,
                            voters.Sid,
                            voters.HouseNumber,
                            voters.StreetPrefix,
                            voters.StreetName,
                            voters.StreetType,
                            voters.StreetSuffix,
                            voters.Unit,
                            voters.UnitNumber,
                            voters.NonStdAddress,
                            voters.NonStdAddressDescription,
                            voters.City,
                            voters.State,
                            voters.Zip,
                            //voters.MailingAddress,
                            //voters.MailingAddress2,
                            //voters.MailingCity,
                            //voters.MailingState,
                            //voters.MailingZip,
                            //voters.MailingCountry,
                            //voters.CountyCode,
                            voters.PrecinctPartId,
                            //voterprecinct.PrecinctPart,
                            //voterprecinct.Precinct,
                            voters.Party,
                            voters.Dl,
                            voters.SSN,
                            //VotersIdRequired = voters.IdRequired,
                            voted.LogCode,
                            logcode.LogDescription,
                            //voted.DateVoted,
                            voted.ActivityDate,
                            //voted.ActivityCode,
                            voted.PollId,
                            //voted.FledVoter,
                            //voted.WrongVoter,
                            //voted.NotTabulated,
                            //voted.BallotNumber,
                            //voted.SignRefused,
                            pollsite.PlaceName,
                            //ballotstyle.BallotStyles.BallotStyleId,
                            //ballotstyle.BallotStyles.BallotStyleName,
                            //ballotstyle.BallotStyles.BallotStyleFileName,
                            voters.BallotStyleId,
                            voters.BallotStyleName,
                            voters.BallotStyleFileName,
                            voters.ModificationType,
                            voters.LastModified
                            //voted.PrintedDate,
                            //voted.Computer,
                            //voted.UserId,
                            //voted.AbsenteeVoterType,
                            //voted.ApplicationIssued,
                            //voted.ApplicationAccepted,
                            //voted.ApplicationRejected,
                            //voted.ApplicationSource,
                            //voted.AppRejectedReason,
                            //voted.IMBOut,
                            //voted.IMBIn,
                            //voted.TempAddress,
                            //voted.AddressType,
                            //voted.BallotRejectedDate,
                            //voted.BallotRejectedReason,
                            //DeliveryAddress1 = voted.AddressLine1,
                            //DeliveryAddress2 = voted.AddressLine2,
                            //DeliveryState = voted.State,
                            //DeliveryCity = voted.City,
                            //DeliveryZip = voted.Zip,
                            //DeliveryCountry = voted.Country,
                            //voted.VoterContactEmail,
                            //voted.VoterContactPhone,
                            //voted.DateIssued,
                            //voted.AbsenteeId,
                            //voted.UocavaApplicationType,
                            //voted.UocavaVoterEmail,
                            //voted.UocavaVoterFax,
                            //voted.BallotDeliveryMethod,
                            //voted.ProvisionalOnly,
                            //voted.SpoiledReason,
                            //voted.BcMailDate,
                            //voted.ServisABSLastModified,
                            //voted.ServisVotedLastModified,
                            //voted.BatchId,
                            //IgnoreRules = voted.ForcePropagate,
                            //VotedIdRequired = idRequired.IdRequired,
                            //voted.CreatedOnDate
                        };

            return query
                .Select(v => new VoterDataModel
                {
                    VoterID = v.VoterId.ToString(),
                    LastName = v.LastName,
                    FirstName = v.FirstName,
                    MiddleName = v.MiddleName,
                    Generation = v.Suffix,
                    Title = v.Title,
                    FullName = String.Concat(
                        v.Title, " ",
                        v.FirstName, " ",
                        v.MiddleName, " ",
                        v.LastName, " ",
                        v.Suffix)
                        .Trim().Replace("  ", " "),
                    DOBYear = v.DateOfBirth,
                    DOBSearch = v.DateOfBirth,
                    Gender = v.Gender,
                    Party = v.Party,
                    DriversLicense = v.Dl,
                    //IDRequired = v.VotersIdRequired,
                    //VotedIdRequired = v.VotedIdRequired,
                    //IDRequired = false,
                    Status = v.VoterStatus,
                    DL = v.Dl,
                    SSN = v.SSN,
                    SID = v.Sid,
                    HouseNumber = v.HouseNumber,
                    StreetPrefix = v.StreetPrefix,
                    StreetName = v.StreetName,
                    StreetType = v.StreetType,
                    StreetSuffix = v.StreetSuffix,
                    Unit = v.Unit,
                    UnitNumber = v.UnitNumber,
                    Address1 = v.NonStdAddress != null && v.NonStdAddress == true ? v.NonStdAddressDescription :
                        String.Concat(
                        v.HouseNumber, " ",
                        v.StreetPrefix, " ",
                        v.StreetName, " ",
                        v.StreetType, " ",
                        v.StreetSuffix)
                        .Trim()
                        .Replace("  ", " "),
                    Address2 = String.Concat(
                        v.Unit, " ",
                        v.UnitNumber)
                        .Trim()
                        .Replace("  ", " "),
                    City = v.City,
                    State = v.State,
                    Zip = v.Zip,
                    NonStandardAddressFlag = (bool)v.NonStdAddress.Value,
                    NonStandardAddressDescription = v.NonStdAddressDescription,
                    //MailingAddress1 = v.MailingAddress,
                    //MailingAddress2 = v.MailingAddress2,
                    //MailingCity = v.MailingCity,
                    //MailingState = v.MailingState,
                    //MailingZip = v.MailingZip,
                    //MailingCSZ = String.Concat(
                    //    v.MailingCity, ", ",
                    //    v.State, " ",
                    //    v.MailingZip)
                    //    .Trim()
                    //    .Replace("  ", " "),
                    //MailingCountry = v.MailingCountry,
                    //County = v.CountyCode,
                    //DeliveryAddress1 = v.DeliveryAddress1,
                    //DeliveryAddress2 = v.DeliveryAddress2,
                    //DeliveryState = v.DeliveryState,
                    //DeliveryCity = v.DeliveryCity,
                    //DeliveryZip = v.DeliveryZip,
                    //DeliveryCountry = v.DeliveryCountry,
                    //TempAddress = v.TempAddress,
                    TempAddress = false,
                    //AddressType = v.AddressType,
                    //PrecinctPartID = v.PrecinctPartId ?? "",

                    // LOG CODE CONDITIONS
                    // If the Voters Modified Type is D or precinct part is missing then use Log Code 17
                    //LogCode = v.LogCode,
                    LogCode = v.ModificationType == "D" || v.PrecinctPartId == null ? 17 : v.LogCode == null ? 1 : v.LogCode,

                    // LOG DESCRIPTION CONDITIONS
                    // If precinct part is missing display "No Precinct"
                    LogDescription = v.PrecinctPartId == null ? "NO PRECINCT" :
                    // or if Modified Type is D then display "Voter Removed"
                    v.ModificationType == "D" ? "VOTER REMOVED" :
                    // or if there is no log code then display "Registered to Vote"
                    v.LogDescription == null ? "REGISTERED TO VOTE" :
                    v.LogDescription.ToUpper(),

                    BallotStyleID = v.BallotStyleId,
                    BallotStyle = v.BallotStyleName,
                    BallotStyleFile = v.BallotStyleFileName,

                    PollID = v.PollId,
                    PollName = v.PlaceName,
                    //ApplicationIssued = v.ApplicationIssued,
                    //ApplicationAccepted = v.ApplicationAccepted,
                    //ApplicationRejected = v.ApplicationRejected,
                    //BallotIssued = v.DateIssued,
                    //BallotPrinted = v.PrintedDate,
                    //AbsenteeId = v.AbsenteeId,
                    //AbsenteeType = v.AbsenteeVoterType,
                    //ApplicationSource = v.ApplicationSource,
                    //UocavaApplicationType = v.UocavaApplicationType,
                    //UocavaVoterEmail = v.UocavaVoterEmail,
                    //UocavaVoterFax = v.UocavaVoterFax,
                    //BallotDeliveryMethod = v.BallotDeliveryMethod,
                    //VotedDate = v.DateVoted,
                    //ComputerID = v.Computer,
                    //BallotNumber = v.BallotNumber,
                    //SignRefused = (bool)v.SignRefused,
                    SignRefused = false,
                    //UserId = v.UserId,
                    //ActivityDate = v.ActivityDate,
                    //ActivityCode = v.ActivityCode,
                    //ProvisionalOnly = v.ProvisionalOnly,
                    ProvisionalOnly = false,
                    //NotTabulated = v.NotTabulated,
                    NotTabulated = false,
                    //ApplicationRejectedReason = v.AppRejectedReason,
                    //SpoiledReasonID = v.SpoiledReason,
                    //BcMailDate = v.BcMailDate,
                    //OutGoingIMB = v.IMBOut,
                    //InComingIMB = v.IMBIn,
                    //Phone = v.VoterContactPhone,
                    //Email = v.VoterContactEmail,
                    //BallotRejectedDate = v.BallotRejectedDate,
                    //BallotRejectedReason = v.BallotRejectedReason,
                    //ServisABSLastModified = v.ServisABSLastModified,
                    //ServisVotedLastModified = v.ServisVotedLastModified,
                    //BatchID = v.BatchId,
                    //CreatedOnDate = v.CreatedOnDate,

                    PartyVisibility = false,
                    OutofCountry = false,
                    BallotSurrendered = false,
                    FledVoter = false,
                    WrongVoter = false
                }
                );
        }

        // Join local and server copies of the Voted Records
        // Return the newest version
        internal IQueryable<VotedQueryModel> JoinVotedRecords(ElectionContext context)
        {
            var query = from voters in context.SosVoters
                        join votedrecord in context.AvVotedRecords
                            on voters.VoterId equals votedrecord.VoterId into votedrecordgroup
                        from votedRecord in votedrecordgroup.DefaultIfEmpty()
                        join votedActivity in context.AvActivities
                            on voters.VoterId equals votedActivity.VoterId into votedactivitygroup
                        from activity in votedactivitygroup.DefaultIfEmpty()
                        select new
                        {
                            voters.VoterId,
                            LogCode = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.LogCode : activity.LogCode,
                            PollId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.PollId : activity.PollId,
                            ApplicationIssued = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationIssued : activity.ApplicationIssued,
                            ApplicationAccepted = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationAccepted : activity.ApplicationAccepted,
                            ApplicationRejected = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationRejected : activity.ApplicationRejected,
                            DateIssued = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.DateIssued : activity.DateIssued,
                            PrintedDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.PrintedDate : activity.PrintedDate,
                            AbsenteeId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AbsenteeId : activity.AbsenteeId,
                            AbsenteeVoterType = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AbsenteeVoterType : activity.AbsenteeVoterType,
                            ApplicationSource = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ApplicationSource : activity.ApplicationSource,
                            UocavaApplicationType = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UocavaApplicationType : activity.UocavaApplicationType,
                            UocavaVoterEmail = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UocavaVoterEmail : activity.UocavaVoterEmail,
                            UocavaVoterFax = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UocavaVoterFax : activity.UocavaVoterFax,
                            BallotDeliveryMethod = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotDeliveryMethod : activity.BallotDeliveryMethod,
                            AddressType = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AddressType : activity.AddressType,
                            AddressLine1 = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AddressLine1 : activity.AddressLine1,
                            AddressLine2 = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AddressLine2 : activity.AddressLine2,
                            State = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.State : activity.State,
                            City = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.City : activity.City,
                            Zip = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.Zip : activity.Zip,
                            Country = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.Country : activity.Country,
                            TempAddress = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.TempAddress : activity.TempAddress,
                            DateVoted = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.DateVoted : activity.DateVoted,
                            Computer = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.Computer : activity.Computer,
                            BallotNumber = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotNumber : activity.BallotNumber,
                            SignRefused = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.SignRefused : activity.SignRefused,
                            UserId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.UserId : activity.UserId,
                            ActivityCode = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ActivityCode : activity.ActivityCode,
                            ActivityDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ActivityDate : activity.ActivityDate,
                            ProvisionalOnly = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ProvisionalOnly : activity.ProvisionalOnly,
                            NotTabulated = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.NotTabulated : activity.NotTabulated,
                            AppRejectedReason = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.AppRejectedReason : activity.AppRejectedReason,
                            SpoiledReason = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.SpoiledReason : activity.SpoiledReason,
                            BcMailDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BcMailDate : activity.BcMailDate,
                            IMBOut = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.IMBOut : activity.IMBOut,
                            IMBIn = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.IMBIn : activity.IMBIn,
                            VoterContactEmail = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.VoterContactEmail : activity.VoterContactEmail,
                            VoterContactPhone = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.VoterContactPhone : activity.VoterContactPhone,
                            BallotRejectedDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotRejectedDate : activity.BallotRejectedDate,
                            BallotRejectedReason = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BallotRejectedReason : activity.BallotRejectedReason,
                            ServisABSLastModified = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ServisABSLastModified : activity.ServisABSLastModified,
                            ServisVotedLastModified = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ServisABSLastModified : activity.ServisABSLastModified,
                            BatchId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.BatchId : activity.BatchId,
                            IgnoreRules = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ForcePropagate : activity.IgnoreRules,
                            CreatedOnDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.CreatedOnDate : activity.CreatedOnDate
                        };

            return query.Select(v => new VotedQueryModel
            {
                VoterId = v.VoterId,
                LogCode = v.LogCode,
                //ElectionId = v.ElectionId,
                //PrecinctPartId = v.PrecinctPartId,
                //BallotStyleId = v.BallotStyleId,
                PollId = v.PollId,
                //CountyCode = v.CountyCode,
                ApplicationIssued = v.ApplicationIssued,
                ApplicationAccepted = v.ApplicationAccepted,
                ApplicationRejected = v.ApplicationRejected,
                AppRejectedReason = v.AppRejectedReason,
                DateIssued = v.DateIssued,
                PrintedDate = v.PrintedDate,
                AbsenteeId = v.AbsenteeId,
                AbsenteeVoterType = v.AbsenteeVoterType,
                ApplicationSource = v.ApplicationSource,
                UocavaApplicationType = v.UocavaApplicationType,
                BallotDeliveryMethod = v.BallotDeliveryMethod,
                UocavaVoterEmail = v.UocavaVoterEmail,
                UocavaVoterFax = v.UocavaVoterFax,
                AddressType = v.AddressType,
                AddressLine1 = v.AddressLine1,
                AddressLine2 = v.AddressLine2,
                City = v.City,
                State = v.State,
                Zip = v.Zip,
                Country = v.Country,
                DateVoted = v.DateVoted,
                Computer = v.Computer,
                BallotNumber = v.BallotNumber,
                SignRefused = v.SignRefused,
                UserId = v.UserId,
                ActivityCode = v.ActivityCode,
                ActivityDate = v.ActivityDate,
                ProvisionalOnly = v.ProvisionalOnly,
                NotTabulated = v.NotTabulated,
                SpoiledReason = v.SpoiledReason,
                BcMailDate = v.BcMailDate,
                ForcePropagate = v.IgnoreRules,
                CreatedOnDate = v.CreatedOnDate,
                TempAddress = v.TempAddress,
                IMBOut = v.IMBOut,
                IMBIn = v.IMBIn,
                BallotRejectedDate = v.BallotRejectedDate,
                BallotRejectedReason = v.BallotRejectedReason,
                ServisABSLastModified = v.ServisABSLastModified,
                ServisVotedLastModified = v.ServisVotedLastModified,
                VoterContactEmail = v.VoterContactEmail,
                VoterContactPhone = v.VoterContactPhone,
                BatchId = v.BatchId
            });
        }

        internal IQueryable<VotedQueryModel> JoinVotedRecordsSimple(ElectionContext context)
        {
            var query = from voters in context.SosVoters
                        join votedrecord in context.AvVotedRecords
                            on voters.VoterId equals votedrecord.VoterId into votedrecordgroup
                        from votedRecord in votedrecordgroup.DefaultIfEmpty()
                        join votedActivity in context.AvActivities
                            on voters.VoterId equals votedActivity.VoterId into votedactivitygroup
                        from activity in votedactivitygroup.DefaultIfEmpty()
                        select new
                        {
                            voters.VoterId,
                            LogCode = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.LogCode : activity.LogCode,
                            PollId = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.PollId : activity.PollId,                            
                            ActivityDate = votedRecord.LastModified > activity.LastModified || activity.LastModified == null ? votedRecord.ActivityDate : activity.ActivityDate                            
                        };

            return query.Select(v => new VotedQueryModel
            {
                VoterId = v.VoterId,
                LogCode = v.LogCode,
                PollId = v.PollId,
                ActivityDate = v.ActivityDate
            });
        }

        internal IQueryable<VoterBallotStyleModel> JoinBallotStyle(ElectionContext context, int? electionType)
        {
            var ballotStyleQuery = context.SosBallotStyles
                .Join(
                context.SosBallotStylePrecincts,
                BallotStyles => BallotStyles.BallotStyleId,
                BSPrecinct => BSPrecinct.BallotStyleId,
                (BallotStyles, BSPrecinct) => new { BallotStyles, BSPrecinct }
                );

            var query = from voters in context.SosVoters
                        join voterballotstyle in ballotStyleQuery
                            on new { voters.PrecinctPartId } equals new { voterballotstyle.BSPrecinct.PrecinctPartId } into voterballotstylegroup
                        from ballotstyle in voterballotstylegroup.DefaultIfEmpty()
                        select new { voters, ballotstyle };

            if (electionType == 1) // Primary
            {
                query = from voters in context.SosVoters
                        join voterballotstyle in ballotStyleQuery
                                on new { voters.PrecinctPartId, voters.Party } equals new { voterballotstyle.BSPrecinct.PrecinctPartId, voterballotstyle.BallotStyles.Party } into voterballotstylegroup
                        from ballotstyle in voterballotstylegroup.DefaultIfEmpty()
                        select new { voters, ballotstyle };
            }

            return query.Select(v => new VoterBallotStyleModel
            {
                VoterId = v.voters.VoterId,
                FirstName = v.voters.FirstName,
                MiddleName = v.voters.MiddleName,
                LastName = v.voters.LastName,
                Title = v.voters.Title,
                Suffix = v.voters.Suffix,
                DateOfBirth = v.voters.DateOfBirth,
                Gender = v.voters.Gender,
                VoterStatus = v.voters.VoterStatus,
                Sid = v.voters.Sid,
                HouseNumber = v.voters.HouseNumber,
                StreetPrefix = v.voters.StreetPrefix,
                StreetName = v.voters.StreetName,
                StreetType = v.voters.StreetType,
                StreetSuffix = v.voters.StreetSuffix,
                Unit = v.voters.Unit,
                UnitNumber = v.voters.UnitNumber,
                NonStdAddress = v.voters.NonStdAddress,
                NonStdAddressDescription = v.voters.NonStdAddressDescription,
                City = v.voters.City,
                State = v.voters.State,
                Zip = v.voters.Zip,
                MailingAddress = v.voters.MailingAddress,
                MailingAddress2 = v.voters.MailingAddress2,
                MailingCity = v.voters.MailingCity,
                MailingState = v.voters.MailingState,
                MailingZip = v.voters.MailingZip,
                MailingCountry = v.voters.MailingCountry,
                CountyCode = v.voters.CountyCode,
                PrecinctPartId = v.voters.PrecinctPartId,
                Party = v.voters.Party,
                Dl = v.voters.Dl,
                SSN = v.voters.SSN,
                IdRequired = v.voters.IdRequired,
                ModificationType = v.voters.ModificationType,
                LastModified = v.voters.LastModified,

                BallotStyleId = v.ballotstyle.BallotStyles.BallotStyleId,
                BallotStyleName = v.ballotstyle.BallotStyles.BallotStyleName,
                BallotStyleFileName = v.ballotstyle.BallotStyles.BallotStyleFileName
            }
            );
        }

        //internal IQueryable<VotedQueryModel> ChooseVotedRecord(ElectionContext context)
        //{
        //    var query = from votedrecord in context.AvVotedRecords
        //                join votedActivity in context.AvActivities
        //                    on votedrecord.VoterId equals votedActivity.VoterId into votedactivitygroup
        //                from activity in votedactivitygroup.DefaultIfEmpty()
        //}

        //internal IQueryable<VoterDataModel> Query(ElectionContext context)
        //{
        //    var ballotStyleQuery = context.SosBallotStyles
        //        .Join(
        //        context.SosBallotStylePrecincts,
        //        BallotStyles => BallotStyles.BallotStyleId,
        //        BSPrecinct => BSPrecinct.BallotStyleId,
        //        (BallotStyles, BSPrecinct) => new { BallotStyles, BSPrecinct }
        //        );
        //    var query = from voters in context.SosVoters
        //                join votedrecord in context.AvVotedRecords
        //                    on voters.VoterId equals votedrecord.VoterId into votedrecordgroup
        //                from voted in votedrecordgroup.DefaultIfEmpty()
        //                join votedlogcode in context.AvLogCodes
        //                    on voted.LogCode equals votedlogcode.LogCode into votedlogcodegroup
        //                from logcode in votedlogcodegroup.DefaultIfEmpty()
        //                join votedpollsite in context.SosLocations
        //                    on voted.PollId equals votedpollsite.PollId into votedpollsitegroup
        //                from pollsite in votedpollsitegroup.DefaultIfEmpty()
        //                join voterballotstyle in ballotStyleQuery
        //                    on new { voters.PrecinctPartId } equals new { voterballotstyle.BSPrecinct.PrecinctPartId } into voterballotstylegroup
        //                from ballotstyle in voterballotstylegroup.DefaultIfEmpty()
        //                select new
        //                {
        //                    voters.VoterId,
        //                    voters.LastName,
        //                    voters.FirstName,
        //                    voters.MiddleName,
        //                    voters.Title,
        //                    voters.Suffix,
        //                    voters.DateOfBirth,
        //                    voters.Gender,
        //                    voters.VoterStatus,
        //                    voters.Sid,
        //                    voters.HouseNumber,
        //                    voters.StreetPrefix,
        //                    voters.StreetName,
        //                    voters.StreetType,
        //                    voters.StreetSuffix,
        //                    voters.Unit,
        //                    voters.UnitNumber,
        //                    voters.NonStdAddress,
        //                    voters.NonStdAddressDescription,
        //                    voters.City,
        //                    voters.State,
        //                    voters.Zip,
        //                    voters.MailingAddress,
        //                    voters.MailingAddress2,
        //                    voters.MailingCity,
        //                    voters.MailingState,
        //                    voters.MailingZip,
        //                    voters.MailingCountry,
        //                    voters.CountyCode,
        //                    voters.PrecinctPartId,
        //                    //voterprecinct.PrecinctPart,
        //                    //voterprecinct.Precinct,
        //                    voters.Party,
        //                    voters.Dl,
        //                    voters.IdRequired,                            
        //                    voted.LogCode,
        //                    logcode.LogDescription,
        //                    voted.DateVoted,
        //                    voted.ActivityDate,
        //                    voted.ActivityCode,
        //                    voted.PollId,
        //                    //voted.FledVoter,
        //                    //voted.WrongVoter,
        //                    voted.NotTabulated,
        //                    voted.BallotNumber,
        //                    pollsite.PlaceName,
        //                    ballotstyle.BallotStyles.BallotStyleId,
        //                    ballotstyle.BallotStyles.BallotStyleName,
        //                    ballotstyle.BallotStyles.BallotStyleFileName,
        //                    voters.ModificationType,
        //                    voters.LastModified,
        //                    voted.PrintedDate,
        //                    voted.Computer,
        //                    voted.AbsenteeVoterType,
        //                    voted.ApplicationIssued,
        //                    voted.ApplicationAccepted,
        //                    voted.ApplicationRejected,
        //                    voted.ApplicationSource,
        //                    voted.AppRejectedReason,
        //                    DeliveryAddress1 = voted.AddressLine1,
        //                    DeliveryAddress2 = voted.AddressLine2,
        //                    DeliveryState = voted.State,
        //                    DeliveryCity = voted.City,
        //                    DeliveryZip = voted.Zip,
        //                    DeliveryCountry = voted.Country,
        //                };

        //    return query
        //        .Select(v => new VoterDataModel
        //        {
        //            VoterID = v.VoterId.ToString(),
        //            LastName = v.LastName,
        //            FirstName = v.FirstName,
        //            MiddleName = v.MiddleName,
        //            Generation = v.Suffix,
        //            Title = v.Title,
        //            FullName = String.Concat(
        //                v.Title, " ",
        //                v.FirstName, " ",
        //                v.MiddleName, " ",
        //                v.LastName, " ",
        //                v.Suffix)
        //                .Trim().Replace("  ", " "),
        //            DOBYear = v.DateOfBirth,
        //            DOBSearch = v.DateOfBirth,
        //            Gender = v.Gender,
        //            Party = v.Party,
        //            DriversLicense = v.Dl,
        //            IDRequired = (bool)v.IdRequired,
        //            Status = v.VoterStatus,
        //            SID = v.Sid,
        //            HouseNumber = v.HouseNumber,
        //            StreetPrefix = v.StreetPrefix,
        //            StreetName = v.StreetName,
        //            StreetType = v.StreetType,
        //            StreetSuffix = v.StreetSuffix,
        //            Unit = v.Unit,
        //            UnitNumber = v.UnitNumber,
        //            Address1 = v.NonStdAddress != null && v.NonStdAddress == true ? v.NonStdAddressDescription :
        //                String.Concat(
        //                v.HouseNumber, " ",
        //                v.StreetPrefix, " ",
        //                v.StreetName, " ",
        //                v.StreetType, " ",
        //                v.StreetSuffix)
        //                .Trim()
        //                .Replace("  ", " "),
        //            Address2 = String.Concat(
        //                v.Unit, " ",
        //                v.UnitNumber)
        //                .Trim()
        //                .Replace("  ", " "),
        //            City = v.City,
        //            State = v.State,
        //            Zip = v.Zip,                    
        //            NonStandardAddressFlag = (bool)v.NonStdAddress,
        //            NonStandardAddressDescription = v.NonStdAddressDescription,
        //            MailingAddress1 = v.MailingAddress,
        //            MailingAddress2 = v.MailingAddress2,
        //            MailingCity = v.MailingCity,
        //            MailingState = v.MailingState,
        //            MailingZip = v.MailingZip,
        //            MailingCSZ = String.Concat(
        //                v.MailingCity, ", ",
        //                v.State, " ",
        //                v.MailingZip)
        //                .Trim()
        //                .Replace("  ", " "),
        //            MailingCountry = v.MailingCountry,
        //            County = v.CountyCode,
        //            DeliveryAddress1 = v.DeliveryAddress1,
        //            DeliveryAddress2 = v.DeliveryAddress2,
        //            DeliveryState = v.DeliveryState,
        //            DeliveryCity = v.DeliveryCity,
        //            DeliveryZip = v.DeliveryZip,
        //            DeliveryCountry = v.DeliveryCountry,
        //            PrecinctPartID = v.PrecinctPartId ?? "",

        //            // LOG CODE CONDITIONS
        //            // If the Voters Modified Type is D or precinct part is missing then use Log Code 17
        //            //LogCode = v.LogCode,
        //            LogCode = v.ModificationType == "D" || v.PrecinctPartId == null ? 17 : v.LogCode == null ? 1 : v.LogCode,

        //            // LOG DESCRIPTION CONDITIONS
        //            // If precinct part is missing display "No Precinct"
        //            LogDescription = v.PrecinctPartId == null ? "No Precinct" :
        //            // or if Modified Type is D then display "Voter Removed"
        //            v.ModificationType == "D" ? "Voter Removed" :
        //            // or if there is no log code then display "Registered to Vote"
        //            v.LogDescription == null ? "Registered to Vote" :
        //            v.LogDescription,

        //            ActivityDate = v.ActivityDate,
        //            ActivityCode = v.ActivityCode,
        //            VotedDate = v.DateVoted,
        //            BallotNumber = v.BallotNumber,
        //            BallotPrinted = v.PrintedDate,
        //            PollID = v.PollId,
        //            PollName = v.PlaceName,
        //            ComputerID = v.Computer,
        //            AbsenteeType = v.AbsenteeVoterType,
        //            ApplicationIssued = v.ApplicationIssued,
        //            ApplicationAccepted = v.ApplicationAccepted,
        //            ApplicationRejected = v.ApplicationRejected,
        //            ApplicationRejectedReason = v.AppRejectedReason,
        //            BallotStyleID = v.BallotStyleId,
        //            BallotStyle = v.BallotStyleName,
        //            BallotStyleFile = v.BallotStyleFileName

        //        }
        //        );
        //}

        internal IQueryable<VoterDataModel> ProvisionalQuery(ElectionContext context)
        {
            return context.AvProvisionals
                .GroupJoin(
                context.AvProvisionalReasons,
                Provisionals => Provisionals.ProvisionalReason,
                Reasons => Reasons.ProvisionalReasonId,
                (p, r) => new { Provisionals = p, Reasons = r.DefaultIfEmpty() }
                )
                .GroupJoin(
                context.SosBallotStyles,
                Provisionals => Provisionals.Provisionals.BallotStyleId,
                BallotStyles => BallotStyles.BallotStyleId,
                (p, b) => new { Provisionals = p, BallotStyles = b.DefaultIfEmpty() }
                )
                .Select(v => new VoterDataModel
                {
                    VoterID = v.Provisionals.Provisionals.VoterId.ToString(),
                    LastName = v.Provisionals.Provisionals.LastName,
                    FirstName = v.Provisionals.Provisionals.FirstName,
                    MiddleName = v.Provisionals.Provisionals.MiddleName,
                    FullName = String.Concat(v.Provisionals.Provisionals.FirstName, " ", v.Provisionals.Provisionals.MiddleName, " ", v.Provisionals.Provisionals.LastName),
                    DOBYear = v.Provisionals.Provisionals.DateOfBirth,
                    Address1 = v.Provisionals.Provisionals.Address,
                    Address2 = v.Provisionals.Provisionals.Address2,
                    City = v.Provisionals.Provisionals.City,
                    State = v.Provisionals.Provisionals.State,
                    Zip = v.Provisionals.Provisionals.Zip,
                    LogDate = v.Provisionals.Provisionals.PrintedDate,
                    LogCode = v.Provisionals.Provisionals.ProvisionalReason,
                    LogDescription = v.Provisionals.Reasons.FirstOrDefault().ProvisionalReason,
                    PollID = v.Provisionals.Provisionals.PollId,
                    ComputerID = v.Provisionals.Provisionals.Computer,
                    BallotStyleID = v.Provisionals.Provisionals.BallotStyleId,
                    BallotStyle = v.BallotStyles.FirstOrDefault().BallotStyleName,
                    BallotStyleFile = v.BallotStyles.FirstOrDefault().BallotStyleFileName
                }
                );
        }

        /// <summary>
        /// Returns a single voter based on a given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ObservableCollection<NMVoter> Single(string id) 
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(ConvertLists(Query(context).IDEquals(id).AsNoTracking().ToList()));
                }
            }
            catch
            {
                return null;
            }
        }

        public List<NMVoter> ListTest()
        {
            using (var context = GetElectionContext())
            {
                return ConvertLists(QueryTest(context).AsNoTracking().ToList());
            }
        }

        public List<VoterDataModel> List()
        {
            using (var context = GetElectionContext())
            {
                return Query(context).AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// Returns a list of voters based on the given search parameters.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ObservableCollection<NMVoter> List(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        //.BirthYearEquals(search.BirthYear)
                        .BirthYearContains(search.BirthYear)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch
            {
                return null;
            }
        }

        public ObservableCollection<NMVoter> List(int siteID, int? machineID, int logCode)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .AtPollSite(siteID)
                        .OnMachine(machineID)
                        .WithLogCode(logCode)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception error)
            {
                return GetVoterErrorList(error);
            }
        }

        public ObservableCollection<NMVoter> List(int siteID, int? machineID, int logCode, int? ballotStyleID)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .AtPollSite(siteID)
                        .OnMachine(machineID)
                        .WithLogCode(logCode)
                        .WithBallotStyle(ballotStyleID)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception error)
            {
                return GetVoterErrorList(error);
            }
        }

        public ObservableCollection<NMVoter> List(ScanHistory.ScanHistory scanSession)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .ListContains(scanSession.VoterIds)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch
            {
                return null;
            }
        }

        public ObservableCollection<NMVoter> BatchList(int siteID, int? machineID, int logCode, int? userId)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .AtPollSite(siteID)
                        .OnMachine(machineID)
                        .WithLogCode(logCode)
                        .IdRequired(false)  // DO NOT INCLUDE ID REQUIRED VOTERS
                        .Where(v => v.UserId == userId || v.UserId == null)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception error)
            {
                return GetVoterErrorList(error);
            }
        }

        public ObservableCollection<NMVoter> BatchList(Guid? batchId, int siteID, int logCode)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext()) 
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .AtPollSite(siteID)
                        .FromBatch(batchId)
                        .WithLogCode(logCode)
                        .IdRequired(false)  // DO NOT INCLUDE ID REQUIRED VOTERS
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception error)
            {
                return GetVoterErrorList(error);
            }
        }

        public ObservableCollection<NMVoter> BatchCompare(List<int> voterIds, int siteID, int? machineID, int logCode, int? userId)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        JoinVotedRecords(context)
                        .Where(v => v.LogCode == logCode &&
                        v.PollId == siteID &&
                        (v.Computer == machineID || v.Computer == null) &&
                        (v.UserId == userId || v.UserId == null) &&
                        voterIds.Contains(v.VoterId))
                        .Select(v => new VoterDataModel()
                        {
                            VoterID = v.VoterId.ToString(),
                            LogCode = v.LogCode,
                            PollID = v.PollId,
                            ComputerID = v.Computer,
                            UserId = v.UserId
                        })
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception error)
            {
                return GetVoterErrorList(error);
            }
        }

        public ObservableCollection<NMVoter> BatchCompare(List<int> voterIds, Guid? batchId, int siteID, int logCode)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        JoinVotedRecords(context)
                        .Where(v => v.LogCode == logCode &&
                        v.PollId == siteID &&
                        (v.BatchId == batchId || v.BatchId == null) &&
                        voterIds.Contains(v.VoterId))
                        .Select(v => new VoterDataModel()
                        {
                            VoterID = v.VoterId.ToString(),
                            LogCode = v.LogCode,
                            PollID = v.PollId,
                            ComputerID = v.Computer,
                            UserId = v.UserId
                        })
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception error)
            {
                return GetVoterErrorList(error);
            }
        }

        /// <summary>
        /// Returns a list of voters ordered by last name from the given search parameters.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ObservableCollection<NMVoter> OrderedList(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        //.BirthYearEquals(search.BirthYear)
                        .BirthYearContains(search.BirthYear)
                        .AsNoTracking()
                        .ToList()
                    )
                    .OrderBy(o => o.Data.LastName)
                    );
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the first 50 voters ordered by last name from the given search parameters.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ObservableCollection<NMVoter> LimitedList(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        //.BirthYearEquals(search.BirthYear)
                        .BirthYearContains(search.BirthYear)
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                        .Take(50)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        public ObservableCollection<NMVoter> LimitedListSimple(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    var results = new ObservableCollection<NMVoter>(
                    ConvertLists(
                        QuerySimple(context)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        //.BirthYearEquals(search.BirthYear)
                        .BirthYearContains(search.BirthYear)
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                        .Take(50)
                        .AsNoTracking()
                        .ToList()
                    ));

                    context.DetachAllEntities();

                    return results;
                }
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        public string RawQuery(VoterSearchModel search)
        {
            return QueryMethods.RawVoterQuery(search, _electionType);
        }

        public ObservableCollection<NMVoter> LimitedListRaw(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    string sql = QueryMethods.RawVoterQuery(search, _electionType);
                    //var result = context.Database.SqlQuery<VoterDataModel>(
                    //    sql).ToList();

                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        context.Database.SqlQuery<VoterDataModel>(sql)
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                        .ToList()
                    ));
                }
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        /// <summary>
        /// Returns pages of voters ordered by last name from the given search parameters.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ObservableCollection<NMVoter> PagedList(VoterSearchModel search, int pageNumber, int pageSize)
        {
            try
            {
                // Reduce page number so page 1 returns the first set of records
                pageNumber--; // SET page_1 = 0

                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        //.BirthYearEquals(search.BirthYear)
                        .BirthYearContains(search.BirthYear)
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                        // Ensure the page number is never less than 0
                        .Skip((pageNumber < 0 ? 0 : pageNumber) * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch
            {
                return null;
            }
        }

        public Task<ObservableCollection<NMVoter>> PagedListAsync(VoterSearchModel search, int pageNumber, int pageSize)
        {
            return PagedListAsync(search, null, pageNumber, pageSize);
        }

        // Site Id added to WHERE clause for preloading federally issued ballots
        public Task<ObservableCollection<NMVoter>> PagedListAsync(VoterSearchModel search, int? pollId, int pageNumber, int pageSize)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Reduce page number so page 1 returns the first set of records
                    pageNumber--; // SET page_1 = 0

                    if (pageNumber < 0) pageNumber = 0;

                    // Create disposable database context
                    using (var context = GetElectionContext())
                    {
                        return new ObservableCollection<NMVoter>(
                        ConvertLists(
                            Query(context)
                            .IDEquals(search.VoterID)
                            .LastNameStartsWith(search.LastName)
                            .FirstNameStartsWith(search.FirstName)
                            //.BirthYearEquals(search.BirthYear)
                            .BirthYearContains(search.BirthYear)
                            .FederalBallot(pollId)
                            .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                            // Unsure the page number is never less than 0
                            .Skip((pageNumber < 0 ? 0 : pageNumber) * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToList()
                        ));
                    }
                }
                catch (Exception error)
                {
                    return GetVoterErrorList(error);
                }
            });
        }

        public ObservableCollection<NMVoter> CanVoteList(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        Query(context)
                        .LogCodeLessthan(5)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        .BirthYearContains(search.BirthYear)
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                        .Take(50)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        public ObservableCollection<NMVoter> CanVoteListSimple(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        QuerySimple(context)
                        .LogCodeLessthan(5)
                        .IDEquals(search.VoterID)
                        .LastNameStartsWith(search.LastName)
                        .FirstNameStartsWith(search.FirstName)
                        .BirthYearContains(search.BirthYear)
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                        .Take(50)
                        .AsNoTracking()
                        .ToList()
                    ));
                }
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        public ObservableCollection<NMVoter> CanVoteListRaw(VoterSearchModel search)
        {
            try
            {
                // Create disposable database context
                using (var context = GetElectionContext())
                {
                    string sql = QueryMethods.RawCanVoteQuery(search);

                    return new ObservableCollection<NMVoter>(
                    ConvertLists(
                        context.Database.SqlQuery<VoterDataModel>(sql)
                        .OrderByDescending(o => o.VoterID)
                        .ToList()
                    ));
                }
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        public Task<ObservableCollection<NMVoter>> ProvisionalOrderedListAsync(VoterSearchModel search, int pollID)
        {
            return Task.Run(() => {
                try
                {
                    using (var context = GetElectionContext())
                    {
                        return new ObservableCollection<NMVoter>(
                        ConvertLists(
                            ProvisionalQuery(context)
                            .IDEquals(search.VoterID)
                            .LastNameStartsWith(search.LastName)
                            .FirstNameStartsWith(search.FirstName)
                            //.BirthYearEquals(search.BirthYear)
                            .BirthYearContains(search.BirthYear)
                            .Where(p => p.PollID == pollID)
                            .OrderBy(o => o.LastName)
                            .AsNoTracking()
                            .ToList()
                        ));
                    }
                }
                catch (Exception error)
                {
                    return GetVoterErrorList(error);
                }
            });
        }

        /// <summary>
        /// Get only active voting dates for the given site
        /// </summary>
        /// <param name="pollID"></param>
        /// <returns></returns>
        public ObservableCollection<string> ActivityDates(int pollID)
        {
            using (var context = GetElectionContext())
            {
                ObservableCollection<string> stringList = new ObservableCollection<string>();

                var dateList = Query(context).Where(p => p.PollID == pollID).Select(ad => new { dayDate = DbFunctions.TruncateTime(ad.VotedDate) }).Distinct().OrderByDescending(o => o.dayDate).AsNoTracking().ToList();

                foreach (var dateItem in dateList)
                {
                    if (dateItem != null)
                    {
                        try
                        {
                            // Convert DateTime? to Short Date String
                            string shortDateItem = DateTime.Parse((dateItem.dayDate.ToString())).ToShortDateString();
                            stringList.Add(shortDateItem);
                        }
                        catch
                        { }
                    }
                }
                return stringList;
            }
        }

        internal ObservableCollection<NMVoter> GetVoterErrorList(Exception error)
        {
            try
            {
                NMVoter votererror = new NMVoter
                {
                    Error = error
                };

                ObservableCollection<NMVoter> errorList = new ObservableCollection<NMVoter>
                {
                    votererror
                };

                return errorList;
            }
            catch
            {
                return null;
            }
        }

        public List<VoterDataModel> ReportList(
            DateTime startDate,
            DateTime endDate,
            List<int> logList,
            List<int> stylesList,
            List<string> partyList,
            List<string> jurisdictionIDList,
            List<int> siteList)
        {
            try
            {
                using (var context = GetElectionContext())
                {
                    var voter = Query(context).Where(vq => vq.ActivityDate > startDate && vq.ActivityDate < endDate);

                    if (logList != null && logList.Count() > 0)
                    {
                        // Get log code description list
                        voter = voter.Where(v => logList.Contains((int)v.LogCode));
                    }
                    else
                    {
                        // Only pull active voters
                        voter = voter.Where(v => v.LogCode != 1);
                    }

                    if (stylesList != null && stylesList.Count() > 0)
                    {

                        voter = voter.Where(v => stylesList.Contains((int)v.BallotStyleID));
                    }

                    if (partyList != null && partyList.Count() > 0)
                    {
                        voter = voter.Where(v => partyList.Contains(v.Party));
                    }

                    if (jurisdictionIDList != null && jurisdictionIDList.Count() > 0)
                    {
                        // Get Jurisdiction Precincts
                        var precinctIDs = context.SosJurisdictionPrecincts.Where(jd => jurisdictionIDList.Contains(jd.JurisdictionId)).Select(jd => jd.PrecinctPartId);
                        voter = voter.Where(v => precinctIDs.Contains(v.PrecinctPartID));
                    }

                    if (siteList != null && siteList.Count() > 0)
                    {
                        // Get site name list
                        voter = voter.Where(v => siteList.Contains((int)v.PollID));
                    }

                    return voter.AsNoTracking().ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<VoterDataModel> ReportListRegistered(
            List<int> logList,
            List<int> stylesList,
            List<string> partyList,
            List<string> jurisdictionIDList,
            List<int> siteList)
        {
            try
            {
                using (var context = GetElectionContext())
                {
                    var voter = Query(context);

                    if (logList != null && logList.Count() > 0)
                    {
                        // Get log code description list
                        voter = voter.Where(v => logList.Contains((int)v.LogCode));
                    }
                    else
                    {
                        // Only pull active voters
                        voter = voter.Where(v => v.LogCode != 1);
                    }

                    if (stylesList != null && stylesList.Count() > 0)
                    {
                        // Get list of ballot style names
                        voter = voter.Where(v => stylesList.Contains((int)v.BallotStyleID));
                    }

                    if (partyList != null && partyList.Count() > 0)
                    {
                        voter = voter.Where(v => partyList.Contains(v.Party));
                    }

                    if (jurisdictionIDList != null && jurisdictionIDList.Count() > 0)
                    {
                        // Get Jurisdiction Precincts
                        var precinctIDs = context.SosJurisdictionPrecincts.Where(jd => jurisdictionIDList.Contains(jd.JurisdictionId)).Select(jd => jd.PrecinctPartId);
                        voter = voter.Where(v => precinctIDs.Contains(v.PrecinctPartID));
                    }

                    if (siteList != null && siteList.Count() > 0)
                    {
                        // Get site name list
                        voter = voter.Where(v => siteList.Contains((int)v.PollID));
                    }

                    return voter.AsNoTracking().ToList();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
