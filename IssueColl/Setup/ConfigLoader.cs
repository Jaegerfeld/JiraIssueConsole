using Jiracoll;
using System.IO;

namespace IssueColl.Setup
{
    class ConfigLoader
    {
        Config config = new Config();
        WorkflowExtractor extractor = new WorkflowExtractor();

        internal Config Config { get => config; set => config = value; }
        internal WorkflowExtractor Extractor { get => extractor; set => extractor = value; }

        public Config LoadWorkflowFromFile(string filename)
        {


            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + filename;

            this.Config.Workflow = extractor.GetWorkflowFromFile(path);

            return this.Config;
        }

        public void SetFilenames(string jsonFilename, string exportFilename)
        {
            this.Config.JsonFileName = jsonFilename;
            this.Config.ExportFileName = exportFilename;
        }
    }
}
