using System;

namespace ABLProfilerConverter
{
    internal class SummaryStatement
    {
        public SummaryStatement(int Id, int LineNumber, int StatementCount, double ActualTime, double TotalCumulativeTime)
        {
            this.Id = Id;
            this.LineNumber = LineNumber;
            this.StatementCount = StatementCount;

            this.TotalActualTime = TimeSpan.FromSeconds(ActualTime);
            this.TotalCumulativeTime = TimeSpan.FromSeconds(TotalCumulativeTime);

            this.ActualTime = this.TotalActualTime / this.StatementCount;
            this.CumulativeTime = this.TotalCumulativeTime / this.StatementCount;
        }

        public int Id { get; set; }
        public int LineNumber { get; set; }
        public int StatementCount { get; set; }
        public TimeSpan ActualTime { get; set; }
        public TimeSpan TotalActualTime { get; set; }
        public TimeSpan CumulativeTime { get; set; }
        public TimeSpan TotalCumulativeTime { get; set; }
        public int ParentId { get; set; }
        public double SessionPercent { get; set; }
        public double PerProcedurePercent { get; set; }
    }
}