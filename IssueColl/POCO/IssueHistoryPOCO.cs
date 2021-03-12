using Atlassian.Jira;
using System;
using System.Collections.Generic;

namespace Jiracoll
{
    class IssueHistoryPOCO
    {
        public string id { get; set; }
        //public string author { get; set; }
        public DateTime created { get; set; }
        public IList<IssueChangeLogItem> items { get; set; }




    }
}
