using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    public class ConsoleSummaryLine
    {
        public string PackageName { get; set; }
        public string ClassName { get; set; }
        public int ExecutableLines { get; set; }
        public int ExecutedLines { get; set; }
        public Double Percentage { get; set; }
    }
}
