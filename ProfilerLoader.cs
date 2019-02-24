using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ABLProfilerConverter
{
    class ProfilerLoader
    {
        private string fileName;
        private StreamReader reader;

        public ProfilerLoader(string fileName)
        {
            this.fileName = fileName;
            FileStream fileStream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read);
            
            reader = File.OpenText(fileName);
        }

        internal ProfileSession GetSession()
        {
            List<string> sessionLines = GetChunk();
            if (sessionLines.Count == 0)
            {
                throw new Exception("No session lines!");
            }
            string sessionLine = sessionLines[0];
            List<string> parts = GetParts(sessionLine);

            ProfileSession profileSession = new ProfileSession()
            {
                VersionNumber = int.Parse(parts[0]),
                TimeStamp = DateTimeFromProgress(parts[1], parts[3]),
                Description = parts[2] as string,
                Username = parts[4] as string
            };

            return profileSession;
        }

        internal List<UserData> GetUserData()
        {
            List<UserData> userDatas = new List<UserData>();
            List<string> userDataLines = GetChunk();
            List<string> parts;
            foreach (string userDataLine in userDataLines)
            {
                parts = GetParts(userDataLine);

                UserData userData = new UserData(double.Parse(parts[0]), parts[1]);
                userDatas.Add(userData);
            }

            return userDatas;
        }

        internal void GetCoverage(ProfileSession profileSession, Dictionary<int, Source> sources)
        {
            List<string> coverageLines = GetChunk();
            List<string> parts;
            while (coverageLines.Count > 0)
            {
                parts = GetParts(coverageLines[0]);
                coverageLines.RemoveAt(0);

                int sourceId = int.Parse(parts[0]);
                if (!string.IsNullOrEmpty(parts[1]))
                {
                    int parentSourceId = sourceId;

                    // skip if parent source unknown
                    if (!sources.ContainsKey(parentSourceId))
                    {
                        //move to next chunk
                        coverageLines = GetChunk();
                        continue;
                    }
                    //find a source for this
                    string sourceName = string.Format("{0} {1}", parts[1], sources[parentSourceId].Name);
                    bool foundSource = false;
                    foreach (Source source in sources.Values)
                    {
                        if (source.Name == sourceName)
                        {
                            foundSource = true;
                            sourceId = source.Id;
                            break;
                        }
                    }
                    if (!foundSource)
                    {
                        Source newSource;
                        if (sourceName.EndsWith(".p")
                            || sourceName.EndsWith(".w"))
                        {
                            newSource = new ProceduralSource(profileSession.GetNextSourceId(), sourceName, 0, "");
                        }
                        else
                        {
                            newSource = new ClassSource(profileSession.GetNextSourceId(), sourceName, 0, "");

                        }

                        sources.Add(newSource.Id, newSource);
                        sourceId = newSource.Id;
                    }
                }

                // this should never happen
                if (!sources.ContainsKey(sourceId))
                {
                    //move to next chunk
                    coverageLines = GetChunk();
                    continue;
                }

                //add the lines
                foreach (string coverageLine in coverageLines)
                {
                    parts = GetParts(coverageLine);
                    ExectableLine exectableLine = new ExectableLine(int.Parse(parts[0]));
                    sources[sourceId].ExecutableLines.Add(exectableLine.LineNumber, exectableLine);
                }

                //move to next chunk
                coverageLines = GetChunk();
            }
        }

        internal List<TraceInformation> GetTraceInformation()
        {
            List<TraceInformation> traceInformations = new List<TraceInformation>();
            List<string> traceInformationLines = GetChunk();
            List<string> parts;
            foreach (string traceInformationLine in traceInformationLines)
            {
                parts = GetParts(traceInformationLine);

                TraceInformation traceInformation = new TraceInformation(int.Parse(parts[0]),
                                                                         int.Parse(parts[1]),
                                                                         double.Parse(parts[2]),
                                                                         double.Parse(parts[3]));
                traceInformations.Add(traceInformation);
            }

            return traceInformations;
        }

        internal Dictionary<int, List<SummaryStatement>> GetSummaryStatements()
        {
            Dictionary<int, List<SummaryStatement>> summaryStatements = new Dictionary<int, List<SummaryStatement>>();
            List<string> summaryStatementLines = GetChunk();
            List<string> parts;
            foreach (string summaryStatementLine in summaryStatementLines)
            {
                parts = GetParts(summaryStatementLine);
                int sourceId = int.Parse(parts[0]);

                SummaryStatement summaryStatement = new SummaryStatement(sourceId,
                                                                         int.Parse(parts[1]),
                                                                         int.Parse(parts[2]),
                                                                         double.Parse(parts[3]),
                                                                         double.Parse(parts[4]));
                if (!summaryStatements.ContainsKey(sourceId))
                {
                    summaryStatements.Add(sourceId, new List<SummaryStatement>());
                }
                summaryStatements[sourceId].Add(summaryStatement);
            }

            return summaryStatements;
        }

        internal Dictionary<Call, int> GetCallTree()
        {
            Dictionary<Call, int> callTree = new Dictionary<Call, int>(); 
            List<string> callTreeLines = GetChunk();
            List<string> parts;
            foreach (string callTreeLine in callTreeLines)
            {
                parts = GetParts(callTreeLine);

                Call call = new Call(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));

                if (callTree.ContainsKey(call))
                {
                    callTree[call] += int.Parse(parts[3]);
                }
                else
                {
                    callTree.Add(call, int.Parse(parts[3]));
                }
            }

            return callTree;
        }

        internal Dictionary<int, Source> GetSources(ProfileSession session)
        {
            Dictionary<int, Source> sources = new Dictionary<int, Source>();

            Source sessionSource = new SessionSource(0, "Session", 0, "")
            {
                CallCount = 1
            };
            sources.Add(sessionSource.Id, sessionSource);

            List<string> sourcesLines = GetChunk();

            List<string> parts;
            Source source;
            foreach (string sourcesLine in sourcesLines)
            {
                parts = GetParts(sourcesLine);

                int id = int.Parse(parts[0]);
                string name = parts[1];
                string listName = parts[2];
                int CRC = int.Parse(parts[3]);

                if (name.EndsWith(".w")
                    || name.EndsWith(".p"))
                {
                    source = new ProceduralSource(id, name, CRC, listName);
                }
                else
                {
                    source = new ClassSource(id, name, CRC, listName);
                }

                if (sources.ContainsKey(source.Id))
                {
                    throw new Exception("Source already exists!");
                }
                sources.Add(source.Id, source);
                if (source.Id > session.LastSourceId)
                {
                    session.LastSourceId = source.Id;
                }
            }

            return sources;
        }

        private DateTime DateTimeFromProgress(string datePart, string timePart)
        {
            string rearrangeDate = datePart; // string.Format("{0}/{1}/{2}", datePart.Substring(3, 2), datePart.Substring(0, 2), datePart.Substring(6, 4));
            DateTime dateTime = DateTime.Parse(string.Format("{0} {1}", rearrangeDate, timePart));
            return dateTime;
        }

        private List<string> GetParts(string sessionLine)
        {
            List<string> parts = new List<string>();
            char[] lineChars = sessionLine.ToCharArray();
            int stringLength = lineChars.Length;

            List<char> chars = new List<char>();
            char c;
            bool stringOpen = false;
            for (int i = 0; i <= stringLength; i++)
            {
                if (i == stringLength)
                {
                    parts.Add(string.Concat(chars));
                    break;
                }

                c = lineChars[i];

                if (c == '"')
                {
                    if (stringOpen)
                    {
                        stringOpen = false;
                        continue;
                    }

                    stringOpen = true;
                    continue;
                }

                if (c == ' ' && !stringOpen)
                {
                    parts.Add(string.Concat(chars));
                    chars = new List<char>();
                    continue;
                }
                chars.Add(c);
            }

            return parts;
        }

        internal List<string> GetChunk()
        {
            List<string> lines = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == ".")
                {
                    break;
                }

                lines.Add(line);
            }

            return lines;
        }
    }
}
