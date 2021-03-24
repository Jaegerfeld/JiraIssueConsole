using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.Report.CFDReport
{


    

    class CFDReport
    {

        string headerLine = "";
        List<CFDReportLine> dayLines = new List<CFDReportLine>();

        public string HeaderLine { get => headerLine; set => headerLine = value; }
        internal List<CFDReportLine> DayLines { get => dayLines; set => dayLines = value; }

        public CFDReport() { }
        

        
    }
}
