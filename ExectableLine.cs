using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    class ExectableLine
    {
        public ExectableLine(int LineNumber)
        {
            this.LineNumber = LineNumber;
        }

        public int LineNumber { get; set; }
        public int TotalCalls { get; set; }
    }
}
