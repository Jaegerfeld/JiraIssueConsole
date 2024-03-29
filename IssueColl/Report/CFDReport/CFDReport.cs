﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.Report.CFDReport
{


    

    class CFDReport
    {

        string headerLine = "";
   
        Dictionary<DateTime, CFDReportLine> dayLines;

        public string HeaderLine { get => headerLine; set => headerLine = value; }
        internal Dictionary<DateTime, CFDReportLine> Daylines { get => dayLines; set => dayLines = value; }

        public CFDReport() { }
        
        public override string ToString()
        {
            string returnString = "";

            returnString += this.headerLine + " \n";



            foreach ( KeyValuePair<DateTime, CFDReportLine> line in this.dayLines)
            {
                
                returnString += line.Value.ToString() + " \n";
            }

            return returnString;
        }
        
    }
}
