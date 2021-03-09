using Jiracoll;
using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.Setup
{
    class Config
    {

        List<WorkflowStep> workflow;
        string jsonFileName;
        String exportFileName;

        public Config()
        {
           
        }

        public string JsonFileName { get => jsonFileName; set => jsonFileName = value; }
        public string ExportFileName { get => exportFileName; set => exportFileName = value; }
        internal List<WorkflowStep> Workflow { get => workflow; set => workflow = value; }
    }
}
