using System;
using System.IO;

namespace IssueColl.Export
{
    class FileExporter
    {



        public void exportToFile(string exportString, string Filename)
        {

            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + Filename + ".csv";
            Console.WriteLine("File exported");
            File.WriteAllText(path, exportString);
        }


    }
}
