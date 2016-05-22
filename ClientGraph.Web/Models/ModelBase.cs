using System;
using System.Collections.Generic;

namespace ClientGraph.Models
{
    public abstract class ModelBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<VersionModel> Versions { get; set; }

        public IList<RelationshipModel> Relationships { get; set; }
    }
}