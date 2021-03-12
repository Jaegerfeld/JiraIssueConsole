using System.Collections.Generic;

namespace Jiracoll
{
    class ChangelogPOCO
    {
        public string startAt { get; set; }
        public string maxResults { get; set; }
        public string total { get; set; }
        public IList<IssueHistoryPOCO> histories { get; set; }

    }
}
