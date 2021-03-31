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
        List<string> notFoundStep = new List<string>();

        internal Config Config { get => config; set => config = value; }
        internal IssueTimesReport Report { get => report; set => report = value; }
        public List<string> NotFoundStep { get => notFoundStep; set => notFoundStep = value; }
        public List<string> DoneStatesList { get => doneStatesList; set => doneStatesList = value; }

        internal IssueTimesReport BuildReport(Config config)
        {
            this.Config = config;


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + config.JsonFileName;

            // load json File
            IssueTimesReport report = new IssueTimesReport();
            string jsonString = File.ReadAllText(path);
            IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);


            report.HeaderLine = this.BuildHeader();


            // every issue is a seperate line in the report csv
            foreach (IssuePOCO issue in JsonContent.issues)
            {
                report.IssueLines.Add(this.BuildLine(issue));
            }

            if (NotFoundStep.Count > 0)
            {
                Console.WriteLine("********Warnings*********\nFound Steps not in Workflow File:\n");
            }
            foreach(string step in NotFoundStep)
            {
                Console.WriteLine(step);
            }
            
            return report;
        }


        /* The header of the report csv has an constant and an flex part.
       Every issue has a group (parent issue), Key, Type, Status, Created Date, Component (maybe EMPTY or NULL), resolution (maybe EMPTY or NULL), */
        private string BuildHeader()
        {
            string header = "";

            header += "Group,Key,Issuetype,Status,Created Date,Component,";
            // every issue may have a First date (beeing in the FIRST status from the config File), and  a Closed Date (last entry in closed state)
            header += "First Date,Closed Date,";

            List<WorkflowStep> statuses = config.Workflow.WorkflowSteps;
            // find the donestate from the config file and mark it


            // adding the given statuses from the config file to the header
            foreach (WorkflowStep status in statuses)
            {
                header += status.Name + ",";
                if (status.DoneState)
                {
                    DoneStatesList.Add(status.Name);
                }
            }

            header += "Resolution";

            //header += System.Environment.NewLine; // finish header
            return header;

        }

        private IssueTimesReportLine BuildLine(IssuePOCO issue)
        {

            string lastName = "";
            string firstName = "";
            //bool foundDate = false;
            

            //Basisc load, without logic total clear data
            IssueTimesReportLine resultLine = new IssueTimesReportLine(issue, config.Workflow.WorkflowSteps);

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
            foreach (WorkflowStep status in this.config.Workflow.WorkflowSteps)
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
                        StatusRich statusTransformation = new StatusRich(item.ToValue, DateTime.Parse(history.created.ToString()), config.Workflow.WorkflowSteps.Find(Status => Status.Name.Equals(item.ToValue)));

                        statusRichList.Add(statusTransformation);
                    }
                }
            }

            //DateTime CloseDate = new DateTime();
            //DateTime FirstDate = new DateTime();
            //DateTime DoneDate = new DateTime();

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
                if(config.Workflow.FirstStatus.Equals(config.Workflow.VeryFirstStep))
                {
                    resultLine.FirstDate = resultLine.CreatedDate;
                }
                
            }
            // sonst Status gefunden, wenn  nicht: immer noch open
            else
            {
                DateTime last;
                resultLine.FirstDate = statusRichList.Last().TimeStamp;
                // wenn es einen Donestatus gibt ist der letzte das Ende Date
                
                // Case 1: <LAST> State found
                //if (statusRichList.Any(p => p.Name == "Done") || statusRichList.Any(p => p.Name == "Abgebrochen"))
                if (statusRichList.Any(p => p.Name.Equals(lastName)))
                {
                    //resultLine.ClosedDate = statusRichList.Max(obj => obj.TimeStamp);

                   StatusRich  doneState = GetFirstDone(statusRichList);
                   resultLine.ClosedDate = doneState.TimeStamp;
                }
           
                else // Case 2: Handling not passed <LAST> but already Done &  Case 3: deprecated States
                {
                    foreach (StatusRich statusRich in statusRichList)
                    {
                        List<DateTime> timestamps = new List<DateTime>();
                        // if the current step is a deprecated status find the alias
                        if (statusRich.Step == null)
                        { 
                            WorkflowStep alias = new WorkflowStep();
                            foreach (WorkflowStep step in config.Workflow.WorkflowSteps)
                            {
                                if (step.Aliases.Contains(statusRich.Name))
                                {
                                    alias = step;
                                }
                            }
                            if (alias.DoneState)
                            {
                                timestamps.Add(statusRich.TimeStamp);                                                              
                            }

                            if(timestamps.Count > 0)
                            {
                                 resultLine.ClosedDate = timestamps.Min(obj => obj);
                            }
                        }
                        // if there is a done state, mark that timestamp
                        else if (statusRich.Step.DoneState)
                        {
                            resultLine.ClosedDate = statusRich.TimeStamp;
                        }
                    }

                }


                if (statusRichList.Any(p => p.Name.Equals(firstName)))
                {
                    resultLine.FirstDate = statusRichList.Min(obj => obj.TimeStamp);
                }

                                

                if (statusRichList.Any(p => p.Name.Equals(firstName)))
                {
                    resultLine.FirstDate = statusRichList.Min(obj => obj.TimeStamp);
                }
                else if(statusRichList.Count <1 || config.Workflow.FirstStatus.Equals( config.Workflow.VeryFirstStep))
                {
                    resultLine.FirstDate = resultLine.CreatedDate;
                }

                // letzter Zeitpunkt: Erstelldatum des Datenabzugs (aka "wann ist heute?")                                                
                last = currentDate;
                DateTime firstTrans = new DateTime();
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
                        foreach (WorkflowStep step in config.Workflow.WorkflowSteps)
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
                    if (DoneStatesList.Contains(statusName))
                    {
                        resultLine.DoneDate = statusTrans.TimeStamp;
                        resultLine.FoundDate = true;
                    }
                    if (!dict.ContainsKey(statusName))
                    {
                        dict.Add(statusName, 0);
                        if (!NotFoundStep.Contains(statusName)) 
                        {
                            NotFoundStep.Add(statusName);
                        }
                    }
                    dict[statusName] += statusTrans.Minutes;
                    firstTrans = statusTrans.TimeStamp;
                }

                // add time for initial Status
                int firstTime = (int) (firstTrans - resultLine.CreatedDate).TotalMinutes;
                WorkflowStep first = config.Workflow.FirstStatus;
                dict[first.Name] += firstTime;


                foreach (KeyValuePair<string, int> pair in dict)
                {
                    resultLine.StatusTimes[pair.Key] = pair.Value;
                }

            }


            return resultLine;
        }

        public StatusRich GetFirstDone(List<StatusRich> statusrichlist)
        {
            StatusRich returnStatus = new StatusRich();

            foreach(StatusRich status in statusrichlist)
            {
                if (status.Step.DoneState)
                {
                    return status;
                }
            }

            return returnStatus;
        }
    }

}
