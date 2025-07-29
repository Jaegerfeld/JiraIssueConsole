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
        List<String> notFoundStep = new List<String>();
       

        internal Config Config { get => config; set => config = value; }
        internal IssueTimesReport Report { get => report; set => report = value; }

        internal IssueTimesReport buildReport(Config config)
        {
            this.Config = config;


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + config.JsonFileName;

            // load json File
            //IssueTimesReport//
            report = new IssueTimesReport();
            string jsonString = File.ReadAllText(path);
            IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);


            report.HeaderLine = this.buildHeader();


            // every issue is a seperate line in the report csv
            foreach (IssuePOCO issue in JsonContent.issues)
            {
                report.IssueLines.Add(this.buildLine(issue));
            }

            if (notFoundStep.Count > 0)
            {
                Console.WriteLine("********Warnings*********\nFound Steps not in Workflow File:\n");
            }
            foreach(string step in notFoundStep)
            {
                Console.WriteLine(step);
            }
            
            return report;
        }


        /* The header of the report csv has an constant and an flex part.
       Every issue has a group (parent issue), Key, Type, Status, Created Date, Component (maybe EMPTY or NULL), resolution (maybe EMPTY or NULL), */
        private string buildHeader()
        {
            string header = "";

            header += "Project,Group,Key,Issuetype,Status,Created Date,Component,Category,";
            // every issue may have a First date (beeing in the FIRST status from the config File), and  a Closed Date (last entry in closed state)
            header += "First Date,Implementation Date,Closed Date,";

            List<WorkflowStep> statuses = config.Workflow.WorkflowSteps;
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
            header += "Resolution";

            //header += System.Environment.NewLine; // finish header
            return header;

        }

        private IssueTimesReportLine buildLine(IssuePOCO issue)
        {

            string lastName = "";
            string firstName = "";
            string implName = "";
            //bool foundDate = false;


            this.report.StatusTransitionList += "\n" + issue.key + ";" + "Created" + ";" + issue.fields.created;
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
            if (issue.fields.project != null)
            {
                string pName = issue.fields.project.name;
                if (pName.Contains(","))
                {
                    pName = pName.Replace(',', ' ');
                }
                resultLine.Project = pName;
            }

            // resolution could be Empty or even NULL(depends on jira version) if the issue is not done
            if (issue.fields.resolution != null)
            {
                resultLine.Resolution = issue.fields.resolution.name;
            }

            // Herausfinden ob das Issue schon begonnen (In Anlysis i.A. ) oder angefangen (completed i.A.) wurde
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
                if (status.Impl)
                {
                    implName = status.Name;
                }
            }

            List<StatusRich> statusRichList = new List<StatusRich>();
            // Alle History einträge im changelog durchlaufen und wenn Eintrag ein Status ist
            // status der richlist hinzufügen

            Boolean implNotReached = true;

            foreach (IssueHistoryPOCO history in issue.changelog.histories)
            {
                foreach (IssueChangeLogItem item in history.items)
                {
                    if (item.FieldName.Equals("status"))
                    {
                        StatusRich statusTransformation = null;
                        WorkflowStep step =  Config.Workflow.getStepbyName(item.ToValue);
                        if (step == null) 
                        {
                            Console.WriteLine("not found step");
                            continue;
                        }
                        try
                        {
                             statusTransformation = new StatusRich(step.Name, DateTime.Parse(history.created.ToString()));
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        //StatusRich statusTransformation = new StatusRich(item.ToValue, DateTime.Parse(history.created.ToString()));
                        if (statusTransformation != null)
                        {
                            statusRichList.Add(statusTransformation);
                        }
                        if (implNotReached)
                        {
                            if (step.Impl)
                            {

                                resultLine.ImplDate = DateTime.Parse(history.created.ToString());
                                implNotReached = false;
                            }
                        }
                    }
                }
            }


            // umsortieren letzter zuerst, desc
            statusRichList.Sort((x, y) => y.TimeStamp.CompareTo(x.TimeStamp));

            // Transitionen einzeln merkken für export
            foreach (StatusRich transStatus in statusRichList)
            {
                this.report.StatusTransitionList += "\n" + issue.key + ";" + transStatus.Name + ";" + transStatus.TimeStamp;
            }
           
            DateTime currentDate = this.config.ReportDate;
            // kein Statuswechsel in History ==> immer noch im initialen Status
            if (statusRichList.Count < 1)
            {
                //DateTime currentDate = new DateTime(2020, 12, 17, 12, 29, 00);

                TimeSpan ts = currentDate - issue.fields.created;
                int minutes = (int)ts.TotalMinutes;

                resultLine.IdleIssue = true;
                resultLine.Idletime = minutes;

                if (config.Workflow.FirstStatus.Equals(config.Workflow.VeryFirstStep))
                {
                    resultLine.FirstDate = resultLine.CreatedDate;
                    
                }
            }
            // sonst Status gefunden, wenn  nicht: immer noch open
            else
            
            {
                DateTime last;

                // wenn es einen Donestatus: suche den echten Donestate .
                // Solange zeitlich rückwärts immer nur donestates kommen, ist es der letzte, sonst gibt es noch kein donedate

                if (statusRichList.Any(p => config.Workflow.DoneWorkflowsteps.Contains(p.Name)))
                {
                    foreach (StatusRich richStatus in statusRichList)
                    {
                        WorkflowStep currentStep = config.Workflow.WorkflowSteps.Find(p => p.Name == richStatus.Name);
                        if (!currentStep.DoneState)
                        {
                            break;
                        }
                        else
                        {
                            resultLine.ClosedDate = richStatus.TimeStamp;
                        }
                    }

                }


                if (statusRichList.Any(p => p.Name.Equals(firstName)))
                {
                    StatusRich treffer = (statusRichList.Find(p => p.Name.Equals(firstName)));
                    resultLine.FirstDate = treffer.TimeStamp;
                    //resultLine.FirstDate = statusRichList.Min(obj => obj.TimeStamp);
                }
                else if (statusRichList.Count >0) //if(statusRichList.Any(p => config.Workflow.DoneWorkflowsteps.Contains(p.Name)) brauchen wir gar nicht, wenn kein first, áber statusrichlist >0 dann erster.
                {
                    StatusRich treffer = statusRichList.Last();
                    resultLine.FirstDate = treffer.TimeStamp;
                }


                //if ( ((((statusRichList.Count < 1))) )
                //{
                //    StatusRich treffer = statusRichList.ElementAt(0);
                //    if (!(treffer.Name.Equals(config.Workflow.VeryFirstStep.Name)))
                //    {
                //        resultLine.FirstDate = treffer.TimeStamp;
                //    }
                //    //resultLine.FirstDate = statusRichList.Min(obj => obj.TimeStamp);
                
                if (statusRichList.Count < 1 || config.Workflow.FirstStatus.Equals(config.Workflow.VeryFirstStep))
                {
                    //resultLine.FirstDate = null;
                }



                // Erster Zeitpunkt: Erstelldatum des Datenabzugs (aka "heute")                                                
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
                    if (doneStatesList.Contains(statusName))
                    {
                        resultLine.DoneDate = statusTrans.TimeStamp;
                        resultLine.FoundDate = true;
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
                    firstTrans = statusTrans.TimeStamp;
                }

                // add time for initial Status
                int firstTime = (int) (firstTrans - resultLine.CreatedDate).TotalMinutes;
                WorkflowStep first = config.Workflow.VeryFirstStep;
                dict[first.Name] += firstTime;


                foreach (KeyValuePair<string, int> pair in dict)
                {
                    resultLine.StatusTimes[pair.Key] = pair.Value;
                }

            }


            return resultLine;
        }
    }

}
