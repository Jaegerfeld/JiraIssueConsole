using Jiracoll;
using System;
using System.Collections.Generic;

namespace IssueColl.Setup
{
    class Config
    {

        List<WorkflowStep> workflow;
        string jsonFileName;
        String exportFileName;
        DateTime reportDate = System.DateTime.Now;

        public Config()
        {
        }

        public string JsonFileName { get => jsonFileName; set => jsonFileName = value; }
        public string ExportFileName { get => exportFileName; set => exportFileName = value; }
        public DateTime ReportDate { get => reportDate; set => reportDate = value; }
        internal List<WorkflowStep> Workflow { get => workflow; set => workflow = value; }
    }
}
