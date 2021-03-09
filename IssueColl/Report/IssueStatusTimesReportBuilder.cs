using IssueColl.Setup;
using System;
using System.Collections.Generic;
using System.Text;

namespace IssueColl.Report
{
    class IssueStatusTimesReportBuilder
    {

        IssueTimesReport report;
        Config config;

        internal Config Config { get => config; set => config = value; }
        internal IssueTimesReport Report { get => report; set => report = value; }

        internal IssueTimesReport buildReport( Config config)
        {
            this.Config = config;
            IssueTimesReport report = new IssueTimesReport();






            return report;
        }
    }
}
