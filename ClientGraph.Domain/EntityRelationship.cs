using System;
using ClientGraph.Domain.Enumerations;

namespace ClientGraph.Domain
{
    public class EntityRelationship
    {
        public EntityType ParentEntityType { get; set; }
        public Guid ParentEntityId { get; set; }
        public string ParentEntityName { get; set; }

        public EntityType ChildEntityType { get; set; }
        public Guid ChildEntityId { get; set; }
        public string ChildEntityName { get; set; }

        public RelationshipType RelationshipType { get; set; }
    }
}