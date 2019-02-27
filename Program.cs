using System;
using System.Collections.Generic;
using System.IO;

namespace ABLProfilerConverter
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("Usage: {inputFile} {cobertura_outputFile} {genericcoverage_outputFile}");
                return -1;
            }
            string fileName = args[0];
            string coberturaOutputFile = args[1];
            string genericCoveragesonarOutputFile = args[2];

            if (!File.Exists(fileName))
            {
                Console.Error.WriteLine($"Profiler input file {fileName} not found!");
                return -2;
            }
            Console.Out.WriteLine($"Processing Profiler file {fileName}...");

            ProfilerLoader profilerLoader = new ProfilerLoader(fileName);

            ProfileSession session = profilerLoader.GetSession();

            Dictionary<int, Source> sources = profilerLoader.GetSources(session);

            Dictionary<Call, int> calltree = profilerLoader.GetCallTree();

            Dictionary<int, List<SummaryStatement>> summaryStatements = profilerLoader.GetSummaryStatements();

            List<TraceInformation> traceInformations = profilerLoader.GetTraceInformation();

            profilerLoader.GetCoverage(session, sources);

            /*ignore userdata chunk (double, character) EventTime Data*/
            List<UserData> userDatas = profilerLoader.GetUserData();

            Analyser analyser = new Analyser();
            analyser.Analyse(session, sources, calltree, summaryStatements);

            CoberturaWriter cuberturaWriter = new CoberturaWriter();
            cuberturaWriter.WriteSession(coberturaOutputFile, session, analyser.Classes, analyser.Procedurals);
            Console.Out.WriteLine($"Wrote cobertura results to {coberturaOutputFile}.");

            GenericCoverageWriter genericCoverageWriter = new GenericCoverageWriter();
            genericCoverageWriter.WriteSession(genericCoveragesonarOutputFile, session, analyser.Classes, analyser.Procedurals);
            Console.Out.WriteLine($"Wrote generic coverage results to {genericCoveragesonarOutputFile}.");

            return 0;
        }
    }
}
