using System;
using System.IO;

namespace IssueColl.Export
{
    class FileExporter
    {



        public void ExportToFile(string exportString, string filename)
        {
            string completeFilename = filename + ".csv";
            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + completeFilename;
            Console.WriteLine("****** Job done ******\n" + completeFilename + " ready");
            File.WriteAllText(path, exportString);
        }


    }
}
