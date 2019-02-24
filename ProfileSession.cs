using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    internal class ProfileSession
    {
        public int VersionNumber { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public int LastSourceId { get; set; }
        public TimeSpan TotalActualTime { get; set; }
        public int totalLinesToCover { get; set; }
        public int totalLinesCovered { get; set; }

        public int GetNextSourceId()
        {
            LastSourceId = LastSourceId + 1;
            return LastSourceId;
        }
    }
}
