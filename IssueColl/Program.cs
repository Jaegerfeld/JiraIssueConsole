﻿using IssueColl.Export;
using IssueColl.Report;
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
            FileExporter fileExporter = new FileExporter();


            if (args.Length == 0 || args[0].Equals("-h"))
            {
                Console.WriteLine("\nSyntax: IssueColl [JsonFilename] [Exportfilename] [WorkflowFileName]\n\n" +
                                  "Options    Filename of the  Workflowconfig.  Default: workflow.txt \n"
                    );
            }
            else

            {
                 
                IssueTimesReport report;
                string workflowname = "workflowname.txt";
                if (args.Length == 3)
                {
                    workflowname = args[2];
                }

                configLoader.SetFilenames(args[0], args[1]);
                Config config = configLoader.LoadWorkflowFromFile(workflowname);

                report = issueStatusTimesReportBuilder.BuildReport(config);

                fileExporter.ExportToFile(report.ToString(), config.ExportFileName + "_IssueTimes");

            }

        }
    }
}
