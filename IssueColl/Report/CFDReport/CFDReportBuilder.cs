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
            Console.WriteLine("Building CFD Report.... ");

            this.Config = config;


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + config.JsonFileName;

         
            string jsonString = File.ReadAllText(path);
            IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);


            returnReport.HeaderLine = this.BuildHeader();

            DateTime lastYear = DateTime.Today.AddYears(-6);
            
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

            int issuesOutOfTime = 0;
            foreach (IssuePOCO issue in issues)
            {
                Dictionary<String, Boolean> usedSteps = getUsedStatusDict();

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
                                var test = usedSteps[item.ToValue];
                                if (!usedSteps[item.ToValue]){ 
                                ((returnLines[justday]).StatusCount[item.ToValue]) += 1;
                                usedSteps[item.ToValue] = true;
                                }
                            }
                            catch (System.Collections.Generic.KeyNotFoundException e)
                            {
                                foreach(WorkflowStep step in config.Workflow.WorkflowSteps)
                                {
                                    if (!returnLines.ContainsKey(justday))
                                    {
                                       issuesOutOfTime++;
                                    }
                                    else if (step.Aliases.Contains(item.ToValue))
                                    {
                                       // Console.WriteLine("gefunden: " + step.Name);
                                        ((returnLines[justday]).StatusCount[step.Name]) += 1;
                                        usedSteps[step.Name] = true;
                                    }
                                  

                                }
                                //Console.WriteLine("Status nicht in WorkflowActions: " + item.ToValue );
                            }
                          //returnLines[day].StatusCount.TryGetValue(item.ToValue);
                        }
                    }
                }

            }
            Console.WriteLine("(CFD) issues out of Timeframe: " + issuesOutOfTime);

            return returnLines;
        }
        public Dictionary<String, Boolean> getUsedStatusDict()
        {
            Dictionary<String, Boolean> returnDict = new Dictionary<String, Boolean>();

            foreach (WorkflowStep step in config.Workflow.WorkflowSteps)
            {
                returnDict.Add(step.Name, false);
            }

            return returnDict;
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
