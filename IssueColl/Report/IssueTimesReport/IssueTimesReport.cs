using System.Collections.Generic;

namespace IssueColl.Report
{
    class IssueTimesReport
    {

        string headerLine = "";
        List<IssueTimesReportLine> issueLines = new List<IssueTimesReportLine>();
        string statusTransitionList = "";

        public IssueTimesReport()
        {
          statusTransitionList = "Key;Transition;Timestamp" ;
        }

        public string HeaderLine { get => headerLine; set => headerLine = value; }
        public string StatusTransitionList { get => statusTransitionList; set => statusTransitionList = value; }
        internal List<IssueTimesReportLine> IssueLines { get => issueLines; set => issueLines = value; }

        public override string ToString()
        {
            string returnString = "";

            returnString += this.headerLine + " \n";

            foreach (IssueTimesReportLine line in this.issueLines)
            {
                returnString += line.ToString() + " \n";
            }

            return returnString;
        }

    }
}
