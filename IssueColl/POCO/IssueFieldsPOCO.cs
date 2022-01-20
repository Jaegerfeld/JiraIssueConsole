using Jiracoll.POCOS;
using System;
using System.Collections.Generic;
namespace Jiracoll
{
    class IssueFieldsPOCO
    {
        public string id { get; set; }

        public DateTime created { get; set; }
        public IssueTypePOCO issuetype { get; set; }
        public IssueStatusPOCO status { get; set; }

        //public IssueCategoryPOCO customfield_11404 { get; set; }
        public IList<IssueCategoryPOCO> customfield_11404 { get; set; }
        public IssueResolutionPOCO resolution { get; set; }
        public IList<IssueComponentsItemPOCO> components { get; set; }

    }
}
