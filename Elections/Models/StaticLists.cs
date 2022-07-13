using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterX.Core.Elections
{
    public class ElectionListsModel
    {
        // Static Lists
        public IEnumerable<ApplicationRejectedReasonModel> ApplicationRejectedReasons { get; set; }
        public IEnumerable<BallotRejectedReasonModel> BallotRejectedReasons { get; set; }
        public IEnumerable<BallotStyleModel> BallotStyles { get; set; }
        public ElectionModel Election { get; set; }
        public IEnumerable<JurisdictionModel> Jurisdictions { get; set; }
        public IEnumerable<LocationModel> Locations { get; set; }
        public IEnumerable<LogCodeModel> LogCodes { get; set; }
        public IEnumerable<PartyModel> Partys { get; set; }
        public IEnumerable<PollWorkerModel> PollWorkers { get; set; }
        public IEnumerable<ProvisionalReasonModel> ProvisionalReasons { get; set; }
        public IEnumerable<SpoiledReasonModel> SpoiledReasons { get; set; }

        public ElectionListsModel()
        {
            ApplicationRejectedReasons = new List<ApplicationRejectedReasonModel>();
            BallotRejectedReasons = new List<BallotRejectedReasonModel>();
            BallotStyles = new List<BallotStyleModel>();
            Election = new ElectionModel();
            Jurisdictions = new List<JurisdictionModel>();
            Locations = new List<LocationModel>();
            LogCodes = new List<LogCodeModel>();
            Partys = new List<PartyModel>();
            PollWorkers = new List<PollWorkerModel>();
            ProvisionalReasons = new List<ProvisionalReasonModel>();
            SpoiledReasons = new List<SpoiledReasonModel>();
        }
    }
}
