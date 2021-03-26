using System;

namespace Jiracoll
{
    class StatusRich
    {
        string name;
        int minutes;
        DateTime timeStamp;
        WorkflowStep step;

        public string Name { get => name; set => name = value; }
        public int Minutes { get => minutes; set => minutes = value; }
        public DateTime TimeStamp { get => timeStamp; set => timeStamp = value; }
        internal WorkflowStep Step { get => step; set => step = value; }

        public StatusRich() { }
        public StatusRich(string name, DateTime transformationDate, WorkflowStep step)
        {
            this.name = name;
            this.timeStamp = transformationDate;
            this.minutes = 0;
            this.step = step;
        }

    }
}
