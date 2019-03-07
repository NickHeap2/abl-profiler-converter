using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ABLProfilerConverter
{
    internal class ConsoleWriter
    {
        internal List<ConsoleSummaryLine> consoleSummaryLines;
        int maxPackageNameWidth = 0;
        int maxClassNameWidth = 0;
        int maxExecutableLinesWidth = 0;
        int maxExecutedLinesWidth = 0;
        int maxPercentageWidth = 0;

        internal void WriteSession(ProfileSession session, List<ClassSource> classes, List<ProceduralSource> procedures)
        {
            consoleSummaryLines = new List<ConsoleSummaryLine>();

            WriteClasses(classes);

            WriteProcedurals(procedures);

            ConsoleSummaryLine averageSummaryLine = new ConsoleSummaryLine();
            averageSummaryLine.PackageName = "Average";
            averageSummaryLine.ClassName = "";
            foreach (ConsoleSummaryLine consoleSummaryLine in consoleSummaryLines)
            {
                averageSummaryLine.ExecutableLines += consoleSummaryLine.ExecutableLines;
                averageSummaryLine.ExecutedLines += consoleSummaryLine.ExecutedLines;
            }
            averageSummaryLine.ExecutableLines = averageSummaryLine.ExecutableLines / consoleSummaryLines.Count;
            averageSummaryLine.ExecutedLines = averageSummaryLine.ExecutedLines / consoleSummaryLines.Count;
            averageSummaryLine.Percentage = Math.Round(averageSummaryLine.ExecutedLines / (averageSummaryLine.ExecutableLines / 100.0), 2);

            ConsoleSummaryLine totalSummaryLine = new ConsoleSummaryLine();
            totalSummaryLine.PackageName = "Total";
            totalSummaryLine.ClassName = "";
            foreach (ConsoleSummaryLine consoleSummaryLine in consoleSummaryLines)
            {
                totalSummaryLine.ExecutableLines += consoleSummaryLine.ExecutableLines;
                totalSummaryLine.ExecutedLines += consoleSummaryLine.ExecutedLines;
            }
            totalSummaryLine.Percentage = Math.Round(totalSummaryLine.ExecutedLines / (totalSummaryLine.ExecutableLines / 100.0), 2);


            consoleSummaryLines.Add(totalSummaryLine);

            consoleSummaryLines.Add(averageSummaryLine);

            //int maxPackageNameWidth = 20;
            //int maxClassNameWidth = 30;
            //int maxExecutableLinesWidth = 16;
            //int maxExecutedLinesWidth = 16;
            //int maxPercentageWidth = 16;
            string headerPackageName = "Package Name";
            string headerClassName = "Class Name";
            string headerExecutableLines = "Executable Lines";
            string headerExecutedLines = "Executed Lines";
            string headerPercentage = "Percentage";

            maxPackageNameWidth = headerPackageName.Length;
            maxClassNameWidth = headerClassName.Length;
            maxExecutableLinesWidth = headerExecutableLines.Length;
            maxExecutedLinesWidth = headerExecutedLines.Length;
            maxPercentageWidth = headerPercentage.Length;
            foreach (var consoleSummaryLine in consoleSummaryLines)
            {
                if (consoleSummaryLine.PackageName.Length > maxPackageNameWidth)
                {
                    maxPackageNameWidth = consoleSummaryLine.PackageName.Length;
                }
                if (consoleSummaryLine.ClassName.Length > maxClassNameWidth)
                {
                    maxClassNameWidth = consoleSummaryLine.ClassName.Length;
                }
                if (consoleSummaryLine.ExecutableLines.ToString().Length > maxExecutableLinesWidth)
                {
                    maxExecutableLinesWidth = consoleSummaryLine.ExecutableLines.ToString().Length;
                }
                if (consoleSummaryLine.ExecutedLines.ToString().Length > maxExecutedLinesWidth)
                {
                    maxExecutedLinesWidth = consoleSummaryLine.ExecutedLines.ToString().Length;
                }
                if (consoleSummaryLine.Percentage.ToString().Length > maxPercentageWidth)
                {
                    maxPercentageWidth = consoleSummaryLine.Percentage.ToString().Length;
                }
            }


            int boxWidth = maxPackageNameWidth + maxClassNameWidth + maxExecutableLinesWidth + maxExecutedLinesWidth + maxPercentageWidth + 16;

            string title = string.Format("{0}", session.TimeStamp);

            Console.Out.WriteLine("+" + new String('-', boxWidth - 2) + "+");
            Console.Out.WriteLine("| " + title + new string(' ', boxWidth - title.Length - 4) + " |");
            WriteDivider();

            StringBuilder headersSB = new StringBuilder();
            headersSB.Append("| ");
            headersSB.Append(headerPackageName);
            headersSB.Append(' ', maxPackageNameWidth - headerPackageName.Length);
            headersSB.Append(" | ");
            headersSB.Append(headerClassName);
            headersSB.Append(' ', maxClassNameWidth - headerClassName.Length);
            headersSB.Append(" | ");
            headersSB.Append(headerExecutableLines);
            headersSB.Append(' ', maxExecutableLinesWidth - headerExecutableLines.Length);
            headersSB.Append(" | ");
            headersSB.Append(headerExecutedLines);
            headersSB.Append(' ', maxExecutedLinesWidth - headerExecutedLines.Length);
            headersSB.Append(" | ");
            headersSB.Append(headerPercentage);
            headersSB.Append(' ', maxPercentageWidth - headerPercentage.Length);
            headersSB.Append(" |");
            Console.Out.WriteLine(headersSB);

            WriteDivider();

            foreach (var consoleSummaryLine in consoleSummaryLines)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("| ");
                sb.Append(consoleSummaryLine.PackageName);
                sb.Append(' ', maxPackageNameWidth - consoleSummaryLine.PackageName.Length);

                if (consoleSummaryLine.PackageName == "Average"
                    || consoleSummaryLine.PackageName == "Total")
                {
                    sb.Append("   ");
                }
                else
                {
                    sb.Append(" | ");
                }

                sb.Append(consoleSummaryLine.ClassName);
                sb.Append(' ', maxClassNameWidth - consoleSummaryLine.ClassName.Length);
                sb.Append(" | ");
                sb.Append(consoleSummaryLine.ExecutableLines);
                sb.Append(' ', maxExecutableLinesWidth - consoleSummaryLine.ExecutableLines.ToString().Length);
                sb.Append(" | ");
                sb.Append(consoleSummaryLine.ExecutedLines);
                sb.Append(' ', maxExecutedLinesWidth - consoleSummaryLine.ExecutedLines.ToString().Length);
                sb.Append(" | ");
                sb.Append(consoleSummaryLine.Percentage);
                sb.Append('%');
                sb.Append(' ', maxPercentageWidth - 1 - consoleSummaryLine.Percentage.ToString().Length);
                sb.Append(" |");
                Console.Out.WriteLine(sb);

                WriteDivider();

            }
        }

        private void WriteDivider()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("+");
            sb.Append('-', maxPackageNameWidth + 2);
            sb.Append("+");
            sb.Append('-', maxClassNameWidth + 2);
            sb.Append("+");
            sb.Append('-', maxExecutableLinesWidth + 2);
            sb.Append("+");
            sb.Append('-', maxExecutedLinesWidth + 2);
            sb.Append("+");
            sb.Append('-', maxPercentageWidth + 2);
            sb.Append("+");
            Console.Out.WriteLine(sb);
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

                ConsoleSummaryLine consoleSummaryLine = new ConsoleSummaryLine();
                consoleSummaryLine.PackageName = "Procedure";
                consoleSummaryLine.ClassName = procedure.ProcedureName;

                var procedureEntries = from ProceduralSource proceduralSource in procedures
                                       where proceduralSource.ProcedureName == procedure.ProcedureName
                                       && proceduralSource.CRC != 0
                                       select proceduralSource;
                foreach (ProceduralSource proceduralSource in procedureEntries)
                {
                    foreach (ExectableLine line in proceduralSource.ExecutableLines.Values)
                    {
                        consoleSummaryLine.ExecutableLines += 1;
                        if (line.TotalCalls > 0)
                        {
                            consoleSummaryLine.ExecutedLines += 1;
                        }
                    }
                }
                consoleSummaryLine.Percentage = Math.Round(consoleSummaryLine.ExecutedLines / (consoleSummaryLine.ExecutableLines / 100.0), 2);

                consoleSummaryLines.Add(consoleSummaryLine);
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
                    ConsoleSummaryLine consoleSummaryLine = new ConsoleSummaryLine();
                    consoleSummaryLine.PackageName = packageName;
                    consoleSummaryLine.ClassName = packageClass.ClassName;

                    var classMethods = from ClassSource classSource in classes
                                       where classSource.PackageName == packageName
                                       && classSource.ClassName == packageClass.ClassName
                                       && classSource.MethodName != string.Empty
                                       select classSource;
                    foreach (ClassSource methodSource in classMethods)
                    {
                        foreach (ExectableLine line in methodSource.ExecutableLines.Values)
                        {
                            consoleSummaryLine.ExecutableLines += 1;
                            if (line.TotalCalls > 0)
                            {
                                consoleSummaryLine.ExecutedLines += 1;
                            }
                        }
                    }
                    consoleSummaryLine.Percentage = Math.Round(consoleSummaryLine.ExecutedLines / (consoleSummaryLine.ExecutableLines / 100.0), 2);

                    consoleSummaryLines.Add(consoleSummaryLine);

                }
            }
        }
    }
}