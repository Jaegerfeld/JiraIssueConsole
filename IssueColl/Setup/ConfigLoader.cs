using Jiracoll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IssueColl.Setup
{
    class ConfigLoader
    {
        Config config = new Config();
        WorkflowExtraxtor extractor = new WorkflowExtraxtor();



        public Config loadWorkflowFromFile()
        {
            
            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "workflow.txt";

            this.config.Workflow = extractor.getWorkflowFromFile(path);

            return this.config;
        }

       public void setFilenames(string jsonFilename, string exportFilename)
        {
            this.config.JsonFileName = jsonFilename;
            this.config.ExportFileName = exportFilename;
        }
    }
}
