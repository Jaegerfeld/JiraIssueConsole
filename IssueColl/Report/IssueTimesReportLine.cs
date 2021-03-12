using Jiracoll;
using System.Collections.Generic;

namespace IssueColl.Report
{

    class IssueTimesReportLine
    {

        string group;
        string key;
        string issuetype;
        string status;
        string createdDate;
        List<string> component;
        string resolution;
        Dictionary<string,int> statusTimes;
        string firstDate;
        string closedDate;
        bool idleIssue = false;
        int idletime = 0; 

        public string Group { get => group; set => group = value; }
        public string Key { get => key; set => key = value; }
        public string Issuetype { get => issuetype; set => issuetype = value; }
        public string Status { get => status; set => status = value; }
        public string CreatedDate { get => createdDate; set => createdDate = value; }
        public List<string> Component { get => component; set => component = value; }
        public string Resolution { get => resolution; set => resolution = value; }
        public Dictionary<string, int> StatusTimes { get => statusTimes; set => statusTimes = value; }
        public string FirstDate { get => firstDate; set => firstDate = value; }
        public string ClosedDate { get => closedDate; set => closedDate = value; }
        public bool IdleIssue { get => idleIssue; set => idleIssue = value; }
        public int Idletime { get => idletime; set => idletime = value; }

        public IssueTimesReportLine() { }


        public IssueTimesReportLine(IssuePOCO issue, List<WorkflowStep> workflow)
        {
            //resultLine += group + "," + issue.key + "," + issue.fields.issuetype.name + "," + issue.fields.status.name + "," + issue.fields.created.ToString() + ",";
            this.group = "";
            this.key = issue.key;
            this.issuetype = issue.fields.issuetype.name;
            this.status = issue.fields.status.name;
            this.createdDate = issue.fields.created.ToString();
            this.component = new List<string>();
            this.statusTimes = new Dictionary<string, int>();

            foreach(WorkflowStep step in workflow)
            {
                this.statusTimes.Add(step.Name, 0);
            }

        }

        public override string ToString()
        {
            string returnstring = "";



            return returnstring;
        }
    }
}
