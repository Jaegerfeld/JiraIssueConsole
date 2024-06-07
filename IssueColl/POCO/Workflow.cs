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
        WorkflowStep implStatus;
        public WorkflowStep VeryFirstStep { get => veryFirstStep; set => veryFirstStep = value; }

        public Workflow() { }

        public WorkflowStep FirstStatus { get => firstStatus; set => firstStatus = value; }
        public WorkflowStep LastStatus { get => lastStatus; set => lastStatus = value; }
        internal List<WorkflowStep> WorkflowSteps { get => workflowsteps; set => workflowsteps = value; }
        internal WorkflowStep ImplStatus { get => implStatus; set => implStatus = value; }

        public WorkflowStep getStepbyName(string name)
        {
            //WorkflowStep step = new WorkflowStep();
            foreach (WorkflowStep step in workflowsteps)
            {
                if (step.Name.Equals(name))
                {
                    return step;
                }
                else foreach (String alias in step.Aliases)
                    {
                        if (alias.Equals(name))
                        {
                            return step;
                        }

                    }
            }

            Console.WriteLine("Not found step:" + name);

            return null;                

        }



        }


}
