using IssueColl.Export;
using IssueColl.Report;
using IssueColl.Report.CFDReport;
using IssueColl.Setup;
using System;

namespace IssueColl
{
    class Program
    {
        static void Main(string[] args)
        {

            ConfigLoader configLoader = new ConfigLoader();
            IssueStatusTimesReportBuilder issueStatusTimesReportBuilder = new IssueStatusTimesReportBuilder();
            CFDReportBuilder cFDReportBuilder = new CFDReportBuilder();
            FileExporter fileExporter = new FileExporter();


            if (args.Length == 0 || args[0].Equals("-h"))
            {
                Console.WriteLine("\nSyntax: IssueColl [JsonFilename] [Exportfilename] [WorkflowFileName]\n\n" +
                                  "Options    Filename of the  Workflowconfig.  Default: workflow.txt \n"
                    );
            }
            else

            {
                 
                IssueTimesReport issueTimeReport;
                CFDReport cFDReport;

                string workflowname = "workflow.txt";
                if (args.Length == 3)
                {
                    workflowname = args[2];
                }

                configLoader.setFilenames(args[0], args[1]);
                Config config = configLoader.loadWorkflowFromFile(workflowname);

                issueTimeReport = issueStatusTimesReportBuilder.buildReport(config);
                cFDReport = cFDReportBuilder.BuildReport(config);

                fileExporter.exportToFile(issueTimeReport.ToString(), config.ExportFileName + "_IssueTimes");
                fileExporter.exportToFile(cFDReport.ToString(), config.ExportFileName + "_CFD");

            }

        }
    }
}
