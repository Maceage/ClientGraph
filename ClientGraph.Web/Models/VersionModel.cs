using System;

namespace ClientGraph.Models
{
    public class VersionModel
    {
        public string VersionId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}