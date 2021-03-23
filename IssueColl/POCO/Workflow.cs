using Jiracoll;
using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.POCO
{
    class Workflow
    {
        List<WorkflowStep> workflowsteps;
        WorkflowStep firstStatus;
        WorkflowStep lastStatus;
        WorkflowStep veryFirstStep;
        public WorkflowStep VeryFirstStep { get => veryFirstStep; set => veryFirstStep = value; }

        public Workflow() { }

        public WorkflowStep FirstStatus { get => firstStatus; set => firstStatus = value; }
        public WorkflowStep LastStatus { get => lastStatus; set => lastStatus = value; }
        internal List<WorkflowStep> WorkflowSteps { get => workflowsteps; set => workflowsteps = value; }
    }


}
