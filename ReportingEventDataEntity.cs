using System;
using System.Collections.Generic;

using net_coolblue_datastore_clients.RavenDb;

namespace ApplicationReportingDataDownloader
{
    public class ReportingEventDataEntity : IDocument<int>
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public DateTime EventTime { get; set; }

        public Dictionary<string, string> Properties { get; set; }
        public int Id { get; set; }
    }
}