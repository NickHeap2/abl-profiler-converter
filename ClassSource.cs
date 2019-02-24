using System;
using System.Collections.Generic;
using System.Text;

namespace ABLProfilerConverter
{
    class ClassSource : Source
    {
        public ClassSource(int Id, string Name, int CRC, string listName) : base(Id, Name, CRC, listName)
        {
            string[] parts = Name.Split(' ');
            if (parts.Length == 1)
            {
                MethodName = "";
                ClassName = parts[0];
            }
            else
            {
                MethodName = parts[0];
                ClassName = parts[1];
            }

            if (ClassName.IndexOf('.') > 0)
            {
                PackageName = ClassName.Substring(0, ClassName.LastIndexOf('.'));
            }

            string[] classParts = ClassName.Split('.');
            StringBuilder sb = new StringBuilder();
            foreach (string classPart in classParts)
            {
                if (sb.Length > 0)
                {
                    sb.Append('\\');
                }
                sb.Append(classPart);
            }
            sb.Append(".cls");

            //FileName = sb.ToString();

            //ClassName = ClassName.Split('.')[0];
        }

        public string PackageName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}
