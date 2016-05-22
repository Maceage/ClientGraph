using System;

namespace ClientGraph.Domain
{
    public class EntityVersion
    {
        public string VersionId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}