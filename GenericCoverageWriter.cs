using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ABLProfilerConverter
{
    internal class GenericCoverageWriter
    {
        TextWriter textWriter;

        internal void WriteSession(string coverageFileName, ProfileSession session, List<ClassSource> classes, List<ProceduralSource> procedures)
        {
            textWriter = new StreamWriter(File.Create(coverageFileName));

            textWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");

            long timestamp = new DateTimeOffset(session.TimeStamp).ToUnixTimeSeconds();

            textWriter.WriteLine(string.Format("<coverage version=\"1\" >"));

            WriteClasses(classes);

            WriteProcedurals(procedures);

            textWriter.WriteLine(string.Format("</coverage>"));

            textWriter.Close();
        }

        private void WriteProcedurals(List<ProceduralSource> procedures)
        {
            var distinctProcedures = procedures
               .Where(w => w.CRC != 0)
               .Select(s => new { s.ProcedureName, s.FileName })
               .Distinct()
               .ToList();

            foreach (var procedure in distinctProcedures)
            {
                textWriter.WriteLine(string.Format("<file path=\"{0}\" >", procedure.FileName));

                SortedList<int, ExectableLine> sortedLines = new SortedList<int, ExectableLine>();

                var procedureEntries = from ProceduralSource proceduralSource in procedures
                                       where proceduralSource.ProcedureName == procedure.ProcedureName
                                       select proceduralSource;
                foreach (ProceduralSource proceduralSource in procedureEntries)
                {
                    foreach (ExectableLine line in proceduralSource.ExecutableLines.Values)
                    {
                        if (!sortedLines.ContainsKey(line.LineNumber))
                        {
                            sortedLines.Add(line.LineNumber, line);
                        }
                    }
                }

                foreach (ExectableLine line in sortedLines.Values)
                {
                    textWriter.WriteLine(string.Format("<lineToCover covered=\"{0}\" lineNumber=\"{1}\" />", (line.TotalCalls > 0), line.LineNumber));
                }
                textWriter.WriteLine("</file>");
            }
        }

        private void WriteClasses(List<ClassSource> classes)
        {
            List<string> distinctPackage = classes
               .Select(s => s.PackageName)
               .Distinct()
               .ToList();

            foreach (string packageName in distinctPackage)
            {
                var distinctClasses = classes
                  //.Where(w => w.PackageName == packageName && w.CRC != 0 && !w.ClassName.Contains("test_") && !w.ClassName.Contains("mock_"))
                  .Where(w => w.PackageName == packageName && w.CRC != 0)
                  .Select(s => new { s.ClassName, s.FileName})
                  .Distinct()
                  .ToList();
                foreach (var packageClass in distinctClasses)
                {
                    textWriter.WriteLine(string.Format("<file path=\"{0}\" >", packageClass.FileName));

                    SortedList<int, ExectableLine> sortedLines = new SortedList<int, ExectableLine>();

                    var classMethods = from ClassSource classSource in classes
                                       where classSource.PackageName == packageName
                                       && classSource.ClassName == packageClass.ClassName
                                       && classSource.MethodName != string.Empty
                                       select classSource;
                    foreach (ClassSource methodSource in classMethods)
                    {
                        foreach (ExectableLine line in methodSource.ExecutableLines.Values)
                        {
                            if (!sortedLines.ContainsKey(line.LineNumber))
                            {
                                sortedLines.Add(line.LineNumber, line);
                            }
                        }
                    }

                    //write all lines
                    foreach (ExectableLine line in sortedLines.Values)
                    {
                        textWriter.WriteLine(string.Format("<lineToCover covered=\"{0}\" lineNumber=\"{1}\" />", (line.TotalCalls > 0), line.LineNumber));
                    }

                    textWriter.WriteLine("</file>");
                }
            }
        }
    }
}