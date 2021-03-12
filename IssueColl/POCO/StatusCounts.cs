using System;
using System.Collections.Generic;

namespace Jiracoll.POCOS
{
    class StatusCounts
    {
        private DateTime date;
        List<WorkflowStep> workflowSteps;

        private Dictionary<WorkflowStep, int> statuscounts = new Dictionary<WorkflowStep, int>();
        public Dictionary<WorkflowStep, int> Statuscounts { get => statuscounts; set => statuscounts = value; }

        public StatusCounts(List<WorkflowStep> statuses)
        {

            foreach (WorkflowStep item in statuses)
            {
                this.Statuscounts.Add(item, 0);
            }
        }


    }
}
