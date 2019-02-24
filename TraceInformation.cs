using System;

namespace ABLProfilerConverter
{
    internal class TraceInformation
    {
        public TraceInformation(int Id, int LineNumber, double ActualTime, double StartTime)
        {
            this.Id = Id;
            this.LineNumber = LineNumber;
            this.ActualTime = TimeSpan.FromSeconds(ActualTime);
            this.StartTime = TimeSpan.FromSeconds(StartTime);
        }

        public int Id { get; set; }
        public int LineNumber { get; set; }
        public TimeSpan ActualTime { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}