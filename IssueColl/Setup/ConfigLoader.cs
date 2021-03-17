using Jiracoll;
using System.IO;

namespace IssueColl.Setup
{
    class ConfigLoader
    {
        Config config = new Config();
        WorkflowExtraxtor extractor = new WorkflowExtraxtor();



        public Config loadWorkflowFromFile(string filename)
        {


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + filename;

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
