using IssueColl.Setup;
using Jiracoll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IssueColl.Report.CFDReport
{
    class CFDReportBuilder
    {
        CFDReport report;
        Config config;

        public CFDReportBuilder()
        {
            
        }

        internal CFDReport Report { get => report; set => report = value; }
        internal Config Config { get => config; set => config = value; }

        public CFDReport buildReport(Config config)
        {
            CFDReport returnReport = new CFDReport();

            this.Config = config;


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + config.JsonFileName;

         
            string jsonString = File.ReadAllText(path);
            IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);


            returnReport.HeaderLine = this.buildHeader();

            DateTime lastYear = DateTime.Today.AddYears(-1);
            
            this.buildDateDict(config,lastYear,DateTime.Today);



            return returnReport;
        } 


        public string buildHeader()
        {
            

            string header = "";

            header += "Group,Key,Issuetype,Status,Created Date,Component,Resolution,";
            // every issue may have a First date (beeing in the FIRST status from the config File), and  a Closed Date (last entry in closed state)

            List<WorkflowStep> statuses = config.Workflow.WorkflowSteps;

            // adding the given statuses from the config file to the header
            foreach (WorkflowStep status in statuses)
            {
                header += status.Name + ",";              
            }
            return header;

        }



        public CFDReportLine buildCFDReportLine()
        {
            CFDReportLine returnLine = new CFDReportLine();




            return returnLine;
        }


        public Dictionary<DateTime, CFDReportLine> buildDateDict(Config config, DateTime startDate, DateTime endDate) 
        {
            Dictionary<DateTime, CFDReportLine> returnDict = new Dictionary<DateTime, CFDReportLine>();

            //testcode ToDo:sss
            DateTime StartDate = Convert.ToDateTime("15-08-2017");
            DateTime EndDate = Convert.ToDateTime("20-08-2017");
            foreach (DateTime day in EachCalendarDay(StartDate, EndDate))
            {
                Console.WriteLine("Date is : " + day.ToString("dd-MM-yyyy"));
            }
            




            return returnDict;

        }


        internal IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
    }
}
