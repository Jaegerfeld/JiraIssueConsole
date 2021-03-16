using Atlassian.Jira;
using IssueColl.Setup;
using Jiracoll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IssueColl.Report
{
    class IssueStatusTimesReportBuilder
    {

        IssueTimesReport report;
        Config config;
        List<string> doneStatesList = new List<string>();

        internal Config Config { get => config; set => config = value; }
        internal IssueTimesReport Report { get => report; set => report = value; }

        internal IssueTimesReport buildReport(Config config)
        {
            this.Config = config;


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + config.JsonFileName;

            // load json File
            IssueTimesReport report = new IssueTimesReport();
            string jsonString = File.ReadAllText(path);
            IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);


            report.HeaderLine = this.buildHeader();


            // every issue is a seperate line in the report csv
            foreach (IssuePOCO issue in JsonContent.issues)
            {
                report.IssueLines.Add(this.buildLine(issue));
            }

            return report;
        }


        /* The header of the report csv has an constant and an flex part.
       Every issue has a group (parent issue), Key, Type, Status, Created Date, Component (maybe EMPTY or NULL), resolution (maybe EMPTY or NULL), */
        private string buildHeader()
        {
            string header = "";

            header += "Group,Key,Issuetype,Status,Created Date,Component,Resolution,";

            List<WorkflowStep> statuses = config.Workflow;
            // find the donestate from the config file and mark it
           

            // adding the given statuses from the config file to the header
            foreach (WorkflowStep status in statuses)
            {
                header += status.Name + ",";
                if (status.DoneState)
                {
                    doneStatesList.Add(status.Name);
                }
            }

            // every issue may have a First date (beeing in the FIRST status from the config File), and  a Closed Date (last entry in closed state)
            header += "First Date,Closed Date";
            //header += System.Environment.NewLine; // finish header
            return header;

        }

        private IssueTimesReportLine buildLine(IssuePOCO issue)
        {
            
            string lastName = "";
            string firstName = "";            
            bool foundDate = false;
            List<String> notFoundStep = new List<String>();

            //Basisc load, without logic total clear data
            IssueTimesReportLine resultLine = new IssueTimesReportLine(issue, config.Workflow);

            // there may be more than obne component
            if (issue.fields.components != null)
            {
                foreach (IssueComponentsItemPOCO item in issue.fields.components)
                {
                    resultLine.Component.Add(item.name);
                }
            }

            // resolution could be Empty or even NULL(depends on jira version) if the issue is not done
            if (issue.fields.resolution != null)
            {
                resultLine.Resolution = issue.fields.resolution.name;
            }

            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (WorkflowStep status in this.config.Workflow)
            {
                dict[status.Name] = 0;
                if (status.Last)
                {
                    lastName = status.Name;
                }
                if (status.First)
                {
                    firstName = status.Name;
                }
            }

            List<StatusRich> statusRichList = new List<StatusRich>();

            foreach (IssueHistoryPOCO history in issue.changelog.histories)
            {
                foreach (IssueChangeLogItem item in history.items)
                {
                    if (item.FieldName.Equals("status"))
                    {
                        StatusRich statusTransformation = new StatusRich(item.ToValue, DateTime.Parse(history.created.ToString()));

                        statusRichList.Add(statusTransformation);
                    }
                }
            }

            DateTime CloseDate = new DateTime();
            DateTime FirstDate = new DateTime();
            DateTime DoneDate = new DateTime();

            // umsortieren letzter zuerst, desc
            statusRichList.Sort((x, y) => y.TimeStamp.CompareTo(x.TimeStamp));

            DateTime currentDate = this.config.ReportDate;
            // kein Statuswechsel in History ==> immer noch im initialen Status
            if (statusRichList.Count < 1)
            {
                //DateTime currentDate = new DateTime(2020, 12, 17, 12, 29, 00);

                TimeSpan ts = currentDate - issue.fields.created;
                int minutes = (int)ts.TotalMinutes;

                resultLine.IdleIssue = true;
                resultLine.Idletime = minutes;
            }
            // sonst Status gefunden, wenn  nicht: immer noch open
            else
            {
                DateTime last;

                // wenn es einen Donestatus gibt ist der letzte das Ende Date
                //if (statusRichList.Any(p => p.Name == "Done") || statusRichList.Any(p => p.Name == "Abgebrochen"))
                if (statusRichList.Any(p => p.Name.Equals(lastName)))
                {
                    CloseDate = statusRichList.Max(obj => obj.TimeStamp);
                }

                if (statusRichList.Any(p => p.Name.Equals(firstName)))
                {
                    FirstDate = statusRichList.Min(obj => obj.TimeStamp);
                }

                // wenn es einen Donestatus gibt ist der letzte das Ende Date
                //if (statusRichList.Any(p => p.Name == "Done") || statusRichList.Any(p => p.Name == "Abgebrochen"))
                if (statusRichList.Any(p => p.Name.Equals(lastName)))
                {
                    CloseDate = statusRichList.Max(obj => obj.TimeStamp);
                }

                if (statusRichList.Any(p => p.Name.Equals(firstName)))
                {
                    FirstDate = statusRichList.Min(obj => obj.TimeStamp);
                }

                // Erster Zeitpunkt: Erstelldatum des Datenabzugs (aka "heute")                                                
                last = currentDate;
                // Dauer eines statusverbleibs: Startdate des nachfolgers - Startdate des betrachteten Status
                foreach (StatusRich statusTrans in statusRichList)
                {
                    TimeSpan ts = last - statusTrans.TimeStamp;
                    statusTrans.Minutes = (int)ts.TotalMinutes;
                    last = statusTrans.TimeStamp;
                    string statusName = "";
                    if (!(dict.ContainsKey(statusTrans.Name)))
                    {
                        statusName = statusTrans.Name;
                        foreach (WorkflowStep step in config.Workflow)
                        {
                            if (step.Aliases.Contains(statusTrans.Name))
                            {
                                statusName = step.Name;
                            }
                        }
                    }
                    else
                    {
                        statusName = statusTrans.Name;
                    }
                    if (doneStatesList.Contains(statusName))
                    {
                        DoneDate = statusTrans.TimeStamp;
                        foundDate = true;
                    }
                    if (!dict.ContainsKey(statusName))
                    {
                        dict.Add(statusName, 0);
                        if (!notFoundStep.Contains(statusName))
                        {
                            notFoundStep.Add(statusName);
                        }
                    }
                    dict[statusName] += statusTrans.Minutes;

                }
                foreach (KeyValuePair<string, int> pair in dict)
                {
                    resultLine.StatusTimes[pair.Key] = pair.Value;
                }

            }


                return resultLine;
        }
    }
       
}
