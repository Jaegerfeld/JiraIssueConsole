﻿using IssueColl.Setup;
using IssueColl.Report;
using IssueColl.Export;
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
            
            
            
            Config config = new Config();
            IssueTimesReport report;
            
                                 
            configLoader.setFilenames(args[0], args[1]);
            config = configLoader.loadWorkflowFromFile();

            report = issueStatusTimesReportBuilder.buildReport();


            fileExporter.exportToFile(report.ToString(), config.ExportFileName + "_IssueTimes");








        }
    }
}
