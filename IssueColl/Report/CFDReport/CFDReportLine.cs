using IssueColl.POCO;
using Jiracoll;
using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.Report.CFDReport
{

    


    class CFDReportLine
    {
        DateTime date;
        Dictionary<string, int> statusCount;

        internal Dictionary<string, int> StatusCount { get => statusCount; set => statusCount = value; }


        public CFDReportLine(DateTime day, Workflow workflow)
        {
            this.date = day;
            this.statusCount = new Dictionary<string, int>();
            
            foreach(WorkflowStep step in workflow.WorkflowSteps)
            {
                this.statusCount.Add(step.Name, 0);
            }
        }


         public override string ToString()
        {
            string returnString = "";

            returnString += this.date.ToShortDateString() + ",";
            foreach(KeyValuePair<string, int> pair in this.statusCount)
            {
                returnString += pair.Value + ",";
            }


            return returnString;
        }
    }
}
