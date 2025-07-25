﻿using Jiracoll;
using System;
using System.Collections.Generic;

namespace IssueColl.Report
{

    class IssueTimesReportLine
    {

        string group;
        string project;
        string key;
        string issuetype;
        string status;
        string category;
        DateTime createdDate;
        List<string> component;
        string resolution;
        Dictionary<string, int> statusTimes;
        DateTime firstDate;
        DateTime closedDate;
        DateTime implDate;
        DateTime doneDate;
        bool idleIssue = false;
        int idletime = 0;
        //List<WorkflowStep> workflow;
        bool foundDate = false;
        List<String> notFoundStep = new List<String>();


        public string Group { get => group; set => group = value; }
        public string Key { get => key; set => key = value; }
        public string Issuetype { get => issuetype; set => issuetype = value; }
        public string Status { get => status; set => status = value; }
        public DateTime CreatedDate { get => createdDate; set => createdDate = value; }
        public List<string> Component { get => component; set => component = value; }
        public string Resolution { get => resolution; set => resolution = value; }
        public Dictionary<string, int> StatusTimes { get => statusTimes; set => statusTimes = value; }
        public DateTime FirstDate { get => firstDate; set => firstDate = value; }
        public DateTime ClosedDate { get => closedDate; set => closedDate = value; }
        public bool IdleIssue { get => idleIssue; set => idleIssue = value; }
        public int Idletime { get => idletime; set => idletime = value; }
        public bool FoundDate { get => foundDate; set => foundDate = value; }
        public DateTime DoneDate { get => doneDate; set => doneDate = value; }
        public List<string> NotFoundStep { get => notFoundStep; set => notFoundStep = value; }
        public DateTime ImplDate { get => implDate; set => implDate = value; }
        public string Category { get => category; set => category = value; }
        public string Project { get => project; set => project = value; }

        public IssueTimesReportLine() { }


        public IssueTimesReportLine(IssuePOCO issue, List<WorkflowStep> workflow)
        {
            //resultLine += group + "," + issue.key + "," + issue.fields.issuetype.name + "," + issue.fields.status.name + "," + issue.fields.created.ToString() + ",";
            this.group = "";
            this.key = issue.key;
            this.issuetype = issue.fields.issuetype.name;
            this.status = issue.fields.status.name;
            this.createdDate = issue.fields.created;
            this.component = new List<string>();
            this.statusTimes = new Dictionary<string, int>();

            foreach (WorkflowStep step in workflow)
            {
                this.statusTimes.Add(step.Name, 0);
            }

        }

        public override string ToString()
        {
            string returnstring = "";
            string sep = ",";

            returnstring += this.project + sep + this.group + sep + this.key + sep + this.issuetype + sep + this.status + sep + this.createdDate + sep;

            foreach (string item in this.component)
            {
                string component  = item.Replace(",", "");
                returnstring += component + "|";
            }
            returnstring += sep + this.resolution + sep;
            if (this.idleIssue)
            {

                if ((this.FirstDate == null) || FirstDate.Equals(new System.DateTime()))
                {
                    returnstring += "" + sep + sep +sep;
                }
                else 
                {
                    returnstring += sep + sep;
                }

                //if ((this.FirstDate == null) || FirstDate.Equals(new System.DateTime()))
                //{
                //    returnstring += sep + sep;
                //    //returnstring += ",";
                //}


                // cause the idle time is the only time we counted, rest are zero by definition
                for (int i = 0; i < (this.statusTimes.Count - 1); i++)
                {
                    returnstring += "0" + sep;
                }

            }
            else
            {
                if ((this.FirstDate == null) || FirstDate.Equals(new System.DateTime()))
                {
                    returnstring += ",";
                }
                else
                {
                    returnstring += FirstDate.ToString() + ",";
                }
                
                if ((this.ImplDate == null) || ImplDate.Equals(new System.DateTime()))
                {
                    returnstring += ",";
                }
                else
                {
                    returnstring += ImplDate.ToString() + ",";
                }
                if ((this.ClosedDate == null) || this.ClosedDate.Equals(new System.DateTime()))
                {
                    if (idleIssue)
                    {
                        returnstring += this.DoneDate.ToString() + ",";
                    }
                    else returnstring += ",";
                }
                else
                {
                    returnstring += this.ClosedDate.ToString() + ",";
                }

                foreach (KeyValuePair<string, int> pair in this.statusTimes)
                {
                    returnstring += pair.Value + ",";
                }

                returnstring += this.resolution;

            }            
            

            //returnstring += System.Environment.NewLine;

            return returnstring;
        }
    }
}
