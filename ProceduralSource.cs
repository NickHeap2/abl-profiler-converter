using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    class ProceduralSource : Source
    {
        public ProceduralSource(int Id, string Name, int CRC, string listName) : base(Id, Name, CRC, listName)
        {
            string[] parts = Name.Split(' ');

            if (CRC > 0)
            {
                ProceduralName = "";
                ProcedureName = Name;
            }
            else if (parts.Length == 1)
            {
                ProceduralName = "";
                ProcedureName = parts[0];
            }
            else if (parts.Length == 2)
            {
                if (parts[0].Contains('\\'))
                {
                    ProceduralName = Name.Substring(Name.LastIndexOf('\\') + 1);
                    ProceduralName = ProceduralName.Substring(0, ProceduralName.Length - 2);
                    ProcedureName = Name;
                }
                else
                {
                    ProceduralName = parts[0];
                    ProcedureName = parts[1];
                }
            }
            else
            {
                if (parts[0].Contains('\\'))
                {
                    ProceduralName = Name.Substring(Name.LastIndexOf('\\') + 1);
                    ProceduralName = ProceduralName.Substring(0, ProceduralName.Length - 2);
                    ProcedureName = Name;
                }
                else
                {
                    ProceduralName = parts[0];
                    ProcedureName = Name.Substring(ProceduralName.Length + 1);
                }
            }

            //FileName = ProcedureName;
        }

        public string ProcedureName { get; set; }
        public string ProceduralName { get; set; }
    }
}
