using Atlassian.Jira;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Jiracoll
{
    public class OwnIssue : IJiraEntity
    {
        public OwnIssue() { }

        [JsonProperty("id")]
        public string Self { get; }
        [JsonProperty("id")]
        public string Key { get; }
        [JsonProperty("key")]
        public string Id { get; }
        [JsonProperty("author")]
        public JiraUser Author { get; }
        [JsonProperty("created")]
        public DateTime CreatedDate { get; }
        [JsonProperty("items")]
        public IEnumerable<IssueChangeLogItem> Items { get; }
    }
}


