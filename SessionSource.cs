using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    class SessionSource : Source
    {
        public SessionSource(int Id, string Name, int CRC, string ListName) : base(Id, Name, CRC, ListName)
        {
        }
    }
}
