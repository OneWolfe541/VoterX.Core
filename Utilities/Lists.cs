using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterX.Core.Utilities
{
    // Enumerates the list of logcodes
    public enum LogCodes : int
    {
        Registered = 1,
        RequestedApplication = 2,
        IssuedApplication = 3,
        RejectedApplication = 4,
        AcceptedApplication = 5,
        IssuedAbsenteeBallot = 6,
        InPerson = 7,
        EarlyVoting = 8,
        ReturnedAbsenteeBallot = 9,
        UnsignedAbsenteeBallot = 10,
        Deceased = 11,
        AfterDeadline = 12,
        Ineligble = 13,
        Undeliverable = 14,
        Provisional = 15,
        VotedAtPolls = 16,
        Removed = 17
    }

    // Converts LogCodes enum into an integer value
    public static class LogCodeConstants
    {
        public const int Registered = (int)LogCodes.Registered;
        public const int RequestedApplication = (int)LogCodes.RequestedApplication;
        public const int IssuedApplication = (int)LogCodes.IssuedApplication;
        public const int RejectedApplication = (int)LogCodes.RejectedApplication;
        public const int AcceptedApplication = (int)LogCodes.AcceptedApplication;
        public const int IssuedAbsenteeBallot = (int)LogCodes.IssuedAbsenteeBallot;
        public const int InPerson = (int)LogCodes.InPerson;
        public const int EarlyVoting = (int)LogCodes.EarlyVoting;
        public const int ReturnedAbsenteeBallot = (int)LogCodes.ReturnedAbsenteeBallot;
        public const int UnsignedAbsenteeBallot = (int)LogCodes.UnsignedAbsenteeBallot;
        public const int Deceased = (int)LogCodes.Deceased;
        public const int AfterDeadline = (int)LogCodes.AfterDeadline;
        public const int Ineligble = (int)LogCodes.Ineligble;
        public const int Undeliverable = (int)LogCodes.Undeliverable;
        public const int Provisional = (int)LogCodes.Provisional;
        public const int VotedAtPolls = (int)LogCodes.VotedAtPolls;
        public const int Removed = (int)LogCodes.Removed;

        public static string ConvertToActivity(LogCodes logCode)
        {
            if (logCode == LogCodes.EarlyVoting)
            {
                return "E";
            }
            else if (logCode == LogCodes.VotedAtPolls)
            {
                return "P";
            }
            else return null;
        }
    }

    public enum VoterLookupStatus
    {
        Eligible = 1,
        Ineligible = 2,
        Spoilable = 3,
        Provisional = 4,
        Deleted = 5,
        Hybrid = 6,
        None = 0
    };

    public static class Lists
    {
    }
}
