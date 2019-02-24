using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABLProfilerConverter
{
    internal class Analyser
    {
        public List<ClassSource> Classes { get; set; }
        public List<ProceduralSource> Procedurals { get; set; }

        public void UpdateCalledTimes(Dictionary<Call, int> calltree, Dictionary<int, Source> sources)
        {
            foreach (Call call in calltree.Keys)
            {
                if (!sources.ContainsKey(call.Callee))
                {
                    throw new Exception("Callee not found!");
                }
                // on on times called
                sources[call.Callee].CallCount += calltree[call];
                // and executed line numbers
                //if (call.Caller != 0)
                //{
                //    if (sources[call.Caller].ExecutableLines.ContainsKey(call.CallerLineNumber))
                //    {
                //        sources[call.Caller].ExecutableLines[call.CallerLineNumber].TotalCalls += calltree[call];
                //    }
                //}
            }
        }

        internal void GetSessionTime(ProfileSession session, Dictionary<int, List<SummaryStatement>> summaryStatements)
        {
            //total session time
            bool foundSessionSummary = false;
            if (summaryStatements.ContainsKey(0))
            {
                foreach (SummaryStatement summaryStatement in summaryStatements[0])
                {
                    if (summaryStatement.LineNumber == 0)
                    {
                        foundSessionSummary = true;
                        session.TotalActualTime = summaryStatement.CumulativeTime;
                        break;
                    }
                }
            }
            if (!foundSessionSummary)
            {
                if (summaryStatements.ContainsKey(1))
                {
                    foreach (SummaryStatement summaryStatement in summaryStatements[1])
                    {
                        if (summaryStatement.LineNumber == 0)
                        {
                            foundSessionSummary = true;
                            session.TotalActualTime = summaryStatement.CumulativeTime;
                            break;
                        }
                    }
                }
            }
            if (!foundSessionSummary)
            {
                throw new Exception("No summary statement for session");
            }
        }

        internal void Analyse(ProfileSession session, Dictionary<int, Source> sources, Dictionary<Call, int> calltree, Dictionary<int, List<SummaryStatement>> summaryStatements)
        {
            UpdateCalledTimes(calltree, sources);
            GetSessionTime(session, summaryStatements);
            GetSourceTimes(session, sources, summaryStatements);

            var classesList = from Source source in sources.Values
                              where source.GetType() == typeof(ClassSource)
                                    && !string.IsNullOrEmpty(source.ListName)
                                    && !source.Name.Contains("test_")
                                    && !source.Name.Contains("mock_")
                              select source;
            this.Classes = classesList.Cast<ClassSource>().ToList<ClassSource>();

            var procedureList = from Source source in sources.Values
                                where source.GetType() == typeof(ProceduralSource)
                                      && !string.IsNullOrEmpty(source.ListName)
                                      && !source.Name.Contains("test_")
                                      && !source.Name.Contains("mock_")
                                      && !source.Name.Contains("\\pctinit")
                                select source;
            this.Procedurals = procedureList.Cast<ProceduralSource>().ToList<ProceduralSource>();

            foreach (ProceduralSource procedureSource in procedureList)
            {
                session.totalLinesToCover += procedureSource.ExecutableLines.Count;
                session.totalLinesCovered += procedureSource.ExecutableLines.Where(l => l.Value.TotalCalls > 0).Count();
            }
            foreach (ClassSource classSource in classesList)
            {
                session.totalLinesToCover += classSource.ExecutableLines.Count;
                session.totalLinesCovered += classSource.ExecutableLines.Where(l => l.Value.TotalCalls > 0).Count();
            }
        }

        private void GetSourceTimes(ProfileSession session, Dictionary<int, Source> sources, Dictionary<int, List<SummaryStatement>> summaryStatements)
        {

            foreach (Source source in sources.Values)
            {
                //fill in parent info
                int spaceLocation = source.Name.IndexOf(' ');
                if (spaceLocation > 0 && source.Name.Length > spaceLocation)
                {
                    string parentName = source.Name.Substring(spaceLocation + 1);
                    foreach (Source parentSource in sources.Values)
                    {
                        if (parentSource.Name == parentName)
                        {
                            source.ParentId = parentSource.Id;
                            source.ListName = parentSource.ListName;
                            break;
                        }
                    }
                }

                /* actual and average time */
                if (summaryStatements.ContainsKey(source.Id))
                {
                    foreach (SummaryStatement summaryStatement in summaryStatements[source.Id])
                    {
                        source.TotalActualTime += summaryStatement.TotalActualTime;
                        if (source.ParentId == 0)
                        {
                            summaryStatement.ParentId = source.Id;
                        }
                        else
                        {
                            summaryStatement.ParentId = source.ParentId;
                        }

                        if (source.CallCount > 0)
                        {
                            source.AverageActualTime = source.TotalActualTime / source.CallCount;
                        }

                        if (source.ExecutableLines.ContainsKey(summaryStatement.LineNumber))
                        {
                            source.ExecutableLines[summaryStatement.LineNumber].TotalCalls = summaryStatement.StatementCount;
                        }
                    }
                }

                //runtime percentages
                source.SessionPercent = source.TotalActualTime * 100.0 / session.TotalActualTime;
                source.PerCallPercent = source.AverageActualTime * 100.0 / session.TotalActualTime;

                /* actual and average time */
                if (summaryStatements.ContainsKey(source.Id))
                {
                    foreach (SummaryStatement summaryStatement in summaryStatements[source.Id])
                    {
                        summaryStatement.SessionPercent = summaryStatement.ActualTime * 100.0 / session.TotalActualTime;
                        if (source.AverageActualTime.TotalMilliseconds > 0)
                        {
                            summaryStatement.PerProcedurePercent = summaryStatement.TotalActualTime / 100.0 / source.CallCount / source.AverageActualTime;
                        }
                    }

                    source.FirstLineNumber = 1;
                    foreach (SummaryStatement summaryStatement in summaryStatements[source.Id])
                    {
                        if (summaryStatement.LineNumber >= 0)
                        {
                            if (summaryStatement.LineNumber == 0)
                            {
                                source.OverheadTime = summaryStatement.ActualTime;
                                source.TotalCumulativeTime = summaryStatement.TotalCumulativeTime;
                            }
                            else
                            {
                                source.FirstLineNumber = summaryStatement.LineNumber;
                                break;
                            }
                        }
                    }
                    source.TotalTime = source.AverageActualTime * source.CallCount;
                }
            }
        }
    }
}
