using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    class UserData
    {
        public UserData(double EventTime, string Data)
        {
            this.EventTime = TimeSpan.FromSeconds(EventTime);
            this.Data = Data;
        }

        public TimeSpan EventTime { get; set; }
        public string Data { get; set; }
    }
}
