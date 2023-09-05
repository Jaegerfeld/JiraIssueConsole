using IssueColl.POCO;
using System;
using System.Collections.Generic;

namespace Jiracoll
{
    /* Definition des Workflows
       pro Zeile ein Workflow, Reihenfolge vertikal ist die Reihgenfolge im Export csv horizontal
       deprecated Status können auf aktuelle gemappt werden. Führender Status ist der aktuellöe auf den gemappt wird.
       Trennzeichen :
       e.g. 
       To Do:Open
       Für die messung der T2M kann ein Start und End Status angegeben werden
       e.g. 
       <First>Open
       <Last>Completed*/
    class WorkflowExtractor
    {
        Workflow currentWorkflow;
        //string workflowFilePath;

        public WorkflowExtractor()
        {

        }

        
        internal Workflow CurrentWorkflow { get => currentWorkflow; set => currentWorkflow = value; }

        public Workflow GetWorkflowFromFile(string pathOfWorkflow)
        {
            Workflow returnWorkflow = new Workflow();
            List<WorkflowStep> steps = new List<WorkflowStep>();


            int counter = 0;
            string line;
            //string path = Directory.GetCurrentDirectory();
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\workflow.txt";
            bool firstStep = false;
            WorkflowStep veryFirstStep = new WorkflowStep();
            string path = pathOfWorkflow;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);

            System.Console.WriteLine("\n\nUsed Workflow: \n");

            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);

                if (line.Contains("<First>"))
                {
                    string name = line.Split('>')[1];
                    WorkflowStep step = steps.Find(item => item.Name == name);
                    (steps.Find(item => item.Name == name)).First = true;
                    returnWorkflow.FirstStatus = step;
                }
                if (line.Contains("<Implementation>"))
                {
                    string name = line.Split('>')[1];
                    WorkflowStep step = steps.Find(item => item.Name == name);
                    (steps.Find(item => item.Name == name)).First = true;
                    returnWorkflow.ImplStatus = step;
                }

                else if (line.Contains("<Last>"))
                {
                    string name = line.Split('>')[1];
                    WorkflowStep step = steps.Find(item => item.Name == name);
                    step.Last = true;
                    returnWorkflow.LastStatus = step;
                    
               
                }

                else if (line.Contains("<Create>"))
                {
                    string name = line.Split('>')[1];
                    WorkflowStep step = steps.Find(item => item.Name == name);
                    (steps.Find(item => item.Name == name)).CreateState = true;
                    returnWorkflow.VeryFirstStep = step;
                }

                else
                {
                    WorkflowStep status;
                    if (line.Contains(":"))
                    {
                        int index = 0;
                        string[] statusArray = line.Split(':');
                        string mainStatus = statusArray[0];

                        // first Entry == current Status
                        status = new WorkflowStep(statusArray[0].Trim(), statusArray[0].Trim());

                        // Entry 2+ == mapped deprecated status
                        for (int i = index; i < statusArray.Length; i++)
                        {
                            status.Aliases.Add(statusArray[i].Trim());
                        }
                        //steps.Add(status);
                    }
                    else
                    {
                        status = new WorkflowStep(line.Trim(), line.Trim());
                    }

                    if(firstStep == false)
                    {
                        veryFirstStep = status;
                        firstStep = true;
                    }
                    returnWorkflow.VeryFirstStep = veryFirstStep;
                    steps.Add(status);
                       
                    
                }
                counter++;
            }
            Boolean ended = false;
            System.Console.WriteLine("\n\n");

            foreach (WorkflowStep step in steps)
            {

                if (step.Last)
                {
                    ended = true;
                }
                if (ended)
                {
                    step.DoneState = true;
                }
            }

            file.Close();
            returnWorkflow.WorkflowSteps = steps;

            this.CurrentWorkflow = returnWorkflow;

            return this.CurrentWorkflow;
        }

    }
}
