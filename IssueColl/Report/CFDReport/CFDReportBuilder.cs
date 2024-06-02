using Atlassian.Jira;
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

        public CFDReport BuildReport(Config config)
        {
            CFDReport returnReport = new CFDReport();

            this.Config = config;


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + config.JsonFileName;

         
            string jsonString = File.ReadAllText(path);
            IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);


            returnReport.HeaderLine = this.BuildHeader();

            DateTime lastYear = DateTime.Today.AddYears(-2);
            
            returnReport.Daylines = this.BuildDateDict(config,lastYear,DateTime.Today);

            returnReport.Daylines = this.BuildCFDReportLines(JsonContent.issues, returnReport.Daylines);


            return returnReport;
        }

  

        public string BuildHeader()
        {
            

            string header = "";

            header += "Day,";
            // every issue may have a First date (beeing in the FIRST status from the config File), and  a Closed Date (last entry in closed state)

            List<WorkflowStep> statuses = config.Workflow.WorkflowSteps;

            // adding the given statuses from the config file to the header
            foreach (WorkflowStep status in statuses)
            {
                header += status.Name + ",";              
            }
            return header;

        }



        public Dictionary<DateTime, CFDReportLine>  BuildCFDReportLines(IList<IssuePOCO> issues,Dictionary<DateTime, CFDReportLine> dayLines )
        {
            Dictionary<DateTime, CFDReportLine> returnLines = dayLines;

            foreach (IssuePOCO issue in issues)
            {
                foreach (IssueHistoryPOCO history in issue.changelog.histories)
                {
                    foreach (IssueChangeLogItem item in history.items)
                    {
                        if (item.FieldName.Equals("status"))
                        {
                            DateTime day = DateTime.Parse(history.created.ToString());

                            DateTime justday = new DateTime(day.Year, day.Month, day.Day);
                            try
                            {
                                ((returnLines[justday]).StatusCount[item.ToValue]) += 1;
                            }
                            catch (System.Collections.Generic.KeyNotFoundException e)
                            {

                                Console.WriteLine("Status nicht in Workflow: " + item.ToValue );
                            }
                          //returnLines[day].StatusCount.TryGetValue(item.ToValue);
                        }
                    }
                }

            }


            return returnLines;
        }


        public Dictionary<DateTime, CFDReportLine> BuildDateDict(Config config, DateTime startDate, DateTime endDate) 
        {
            Dictionary<DateTime, CFDReportLine> returnDict = new Dictionary<DateTime, CFDReportLine>();

         
            foreach (DateTime day in EachCalendarDay(startDate, endDate))
            {
                //Console.WriteLine("Date is : " + day.ToString("dd-MM-yyyy"));
                CFDReportLine line = new CFDReportLine(day,this.config.Workflow);
                returnDict.Add(day,line);
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
