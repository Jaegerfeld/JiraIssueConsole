using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.Report
{
    
    class IssueTimesReportLine
    {

        string group;
        string key;
        string issuetype;
        string status;
        string creatadDate;
        string component;
        string resolution;
        List<string> statusTimes; 

        public string Group { get => group; set => group = value; }
        public string Key { get => key; set => key = value; }
        public string Issuetype { get => issuetype; set => issuetype = value; }
        public string Status { get => status; set => status = value; }
        public string CreatadDate { get => creatadDate; set => creatadDate = value; }
        public string Component { get => component; set => component = value; }
        public string Resolution { get => resolution; set => resolution = value; }
        public List<string> StatusTimes { get => statusTimes; set => statusTimes = value; }

        public IssueTimesReportLine() { }

        public override string ToString()
        {
            string returnstring = "";



            return returnstring;
        }
    }
}
