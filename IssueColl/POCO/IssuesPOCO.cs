using System.Collections.Generic;

namespace Jiracoll
{
    class IssuesPOCO
    {
        public string expand { get; set; }
        public string startAt { get; set; }
        public string maxResults { get; set; }
        public string total { get; set; }
        public IList<IssuePOCO> issues { get; set; }
    }
}
