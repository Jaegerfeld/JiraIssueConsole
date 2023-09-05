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
        WorkflowStep implStatus;
        WorkflowStep veryFirstStep;
        public WorkflowStep VeryFirstStep { get => veryFirstStep; set => veryFirstStep = value; }

        public Workflow() { }

        public WorkflowStep FirstStatus { get => firstStatus; set => firstStatus = value; }
        public WorkflowStep LastStatus { get => lastStatus; set => lastStatus = value; }
        internal List<WorkflowStep> WorkflowSteps { get => workflowsteps; set => workflowsteps = value; }
        internal WorkflowStep ImplStatus { get => implStatus; set => implStatus = value; }

        public WorkflowStep GetStatus(string name)
        {

            WorkflowStep returnStep = new WorkflowStep();
            this.workflowsteps.Find(Status => Status.Name.Equals(name));





            return returnStep;
        }

        public WorkflowStep GetAlias(string name)
        {
            WorkflowStep returnStep = new WorkflowStep();


            foreach(WorkflowStep step in this.WorkflowSteps)
            {
                if (step.Aliases.Contains(name))
                {
                    return step;
                }
            }


           

            return returnStep;
        }
    }


}
