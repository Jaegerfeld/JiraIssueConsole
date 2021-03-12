using Jiracoll;
using System.Collections.Generic;
using Jiracoll.POCOS;

namespace IssueColl.Report
{
    class IssueTimesReport
    {

        string headerLine = "";
        List<IssueTimesReportLine> issueLines = new List<IssueTimesReportLine>();
      

        public IssueTimesReport()
        {
           
        }

        public string HeaderLine { get => headerLine; set => headerLine = value; }
        internal List<IssueTimesReportLine> IssueLines { get => issueLines; set => issueLines = value; }

        public override string ToString()
        {
            string returnString = "";




            return returnString;
        }

    }
}
