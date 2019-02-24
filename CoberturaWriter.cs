using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ABLProfilerConverter
{
    internal class CoberturaWriter
    {
        TextWriter textWriter;

        internal void WriteSession(string coverageFileName, ProfileSession session, List<ClassSource> classes, List<ProceduralSource> procedures)
        {
            textWriter = new StreamWriter(File.Create(coverageFileName));

            textWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            long timestamp = new DateTimeOffset(session.TimeStamp).ToUnixTimeSeconds();

            //textWriter.WriteLine("<coverage line-rate=\"0\" branch-rate=\"1\" version=\"1.9\" timestamp=\"1550766376\" lines-covered=\"0\" lines-valid=\"9\" branches-covered=\"0\" branches-valid=\"0\">");
            textWriter.WriteLine(string.Format("<coverage timestamp=\"{0}\" version=\"1.9\" lines-covered=\"{1}\" lines-valid=\"{2}\" branches-covered=\"0\" branches-valid=\"0\" >", timestamp, session.totalLinesCovered, session.totalLinesToCover));

            textWriter.WriteLine("<sources/>");

            textWriter.WriteLine("<packages>");

            WriteClasses(classes);

            WriteProcedurals(procedures);

            textWriter.WriteLine("</packages>");
            textWriter.WriteLine("</coverage>");
            textWriter.Close();
        }

        private void WriteProcedurals(List<ProceduralSource> procedures)
        {

            textWriter.WriteLine("<package name=\"Procedures\" >");

            textWriter.WriteLine("<classes>");

            var distinctProcedures = procedures
               //.Where(w => w.CRC != 0 && !w.ProcedureName.Contains("test_") && !w.ProcedureName.Contains("mock_") && !w.ProcedureName.Contains("\\pctinit"))
               .Where(w => w.CRC != 0)
               .Select(s => new { s.ProcedureName, s.FileName })
               .Distinct()
               .ToList();

            foreach (var procedure in distinctProcedures)
            {
                textWriter.WriteLine(string.Format("<class name=\"{0}\" filename=\"{1}\" >", procedure.ProcedureName, procedure.FileName));

                SortedList<int, ExectableLine> sortedLines = new SortedList<int, ExectableLine>();

                textWriter.WriteLine("<methods>");
                var procedureEntries = from ProceduralSource proceduralSource in procedures
                                       where proceduralSource.ProcedureName == procedure.ProcedureName
                                       select proceduralSource;
                foreach (ProceduralSource proceduralSource in procedureEntries)
                {
                    textWriter.WriteLine(string.Format("<method name=\"{0}\" signature=\"()\" >", proceduralSource.ProceduralName));
                    textWriter.WriteLine("<lines>");

                    foreach (ExectableLine line in proceduralSource.ExecutableLines.Values)
                    {
                        textWriter.WriteLine(string.Format("<line number=\"{0}\" hits=\"{1}\" branch=\"False\" />", line.LineNumber, line.TotalCalls));
                        if (!sortedLines.ContainsKey(line.LineNumber))
                        {
                            sortedLines.Add(line.LineNumber, line);
                        }
                    }
                    textWriter.WriteLine("</lines>");
                    textWriter.WriteLine("</method>");
                }
                textWriter.WriteLine("</methods>");

                textWriter.WriteLine("<lines>");
                foreach (ExectableLine line in sortedLines.Values)
                {
                    textWriter.WriteLine(string.Format("<line number=\"{0}\" hits=\"{1}\" branch=\"False\" />", line.LineNumber, line.TotalCalls));
                }
                textWriter.WriteLine("</lines>");

                textWriter.WriteLine("</class>");
            }
            
            textWriter.WriteLine("</classes>");

            textWriter.WriteLine("</package>");
        }

        private void WriteClasses(List<ClassSource> classes)
        {
            List<string> distinctPackage = classes
               //.Where(w => !w.ClassName.Contains("test_") && !w.ClassName.Contains("mock_"))
               .Select(s => s.PackageName)
               .Distinct()
               .ToList();

            foreach (string packageName in distinctPackage)
            {
                textWriter.WriteLine(string.Format("<package name=\"{0}\" >", packageName));
                textWriter.WriteLine("<classes>");

                var distinctClasses = classes
                  //.Where(w => w.PackageName == packageName && w.CRC != 0 && !w.ClassName.Contains("test_") && !w.ClassName.Contains("mock_"))
                  .Where(w => w.PackageName == packageName && w.CRC != 0)
                  .Select(s => new { s.ClassName, s.FileName})
                  .Distinct()
                  .ToList();
                foreach (var packageClass in distinctClasses)
                {
                    textWriter.WriteLine(string.Format("<class name=\"{0}\" filename=\"{1}\" >", packageClass.ClassName, packageClass.FileName));

                    SortedList<int, ExectableLine> sortedLines = new SortedList<int, ExectableLine>();

                    var classMethods = from ClassSource classSource in classes
                                       where classSource.PackageName == packageName
                                       && classSource.ClassName == packageClass.ClassName
                                       && classSource.MethodName != string.Empty
                                       select classSource;
                    foreach (ClassSource methodSource in classMethods)
                    {
                        textWriter.WriteLine("<methods>");

                        textWriter.WriteLine(string.Format("<method name=\"{0}\" signature=\"()\" >", methodSource.MethodName));

                        //write method lines
                        textWriter.WriteLine("<lines>");
                        foreach (ExectableLine line in methodSource.ExecutableLines.Values)
                        {
                            textWriter.WriteLine(string.Format("<line number=\"{0}\" hits=\"{1}\" branch=\"False\" />", line.LineNumber, line.TotalCalls));
                            if (!sortedLines.ContainsKey(line.LineNumber))
                            {
                                sortedLines.Add(line.LineNumber, line);
                            }
                        }
                        textWriter.WriteLine("</lines>");

                        textWriter.WriteLine("</method>");

                        textWriter.WriteLine("</methods>");
                    }

                    //write all lines
                    textWriter.WriteLine("<lines>");
                    foreach (ExectableLine line in sortedLines.Values)
                    {
                        textWriter.WriteLine(string.Format("<line number=\"{0}\" hits=\"{1}\" branch=\"False\" />", line.LineNumber, line.TotalCalls));
                    }
                    textWriter.WriteLine("</lines>");

                    textWriter.WriteLine("</class>");
                }

                textWriter.WriteLine("</classes>");
                textWriter.WriteLine("</package>");
            }
        }
    }
}