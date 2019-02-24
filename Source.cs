using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    class Source
    {
        public Source(int Id, string Name, int CRC, string ListName)
        {
            this.Id = Id;
            this.Name = Name;
            this.ExecutableLines = new Dictionary<int, ExectableLine>();
            this.TotalActualTime = new TimeSpan();
            this.AverageActualTime = new TimeSpan();
            this.OverheadTime = new TimeSpan();
            this.TotalCumulativeTime = new TimeSpan();
            this.CRC = CRC;
            this.ListName = ListName;
            this.FileName = ListName;

            //if (string.IsNullOrEmpty(this.ListName))
            //{
            //    Console.WriteLine("empty");
            //}
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CallCount { get; set; }
        public string ListName { get; set; }
        public int CRC { get; set; }
        public int ParentId { get; set; }
        public int FirstLineNumber { get; set; }
        public string FileName { get; set; }

        public TimeSpan TotalActualTime { get; set; }
        public TimeSpan AverageActualTime { get; set; }
        public TimeSpan OverheadTime { get; set; }
        public TimeSpan TotalCumulativeTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public double SessionPercent { get; set; }
        public double PerCallPercent { get; set; }

        public Dictionary<int, ExectableLine> ExecutableLines { get; set; }
    }
}
