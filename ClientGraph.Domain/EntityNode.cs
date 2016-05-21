using System;
using ClientGraph.Domain.Enumerations;

namespace ClientGraph.Domain
{
    public class EntityNode
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public EntityType Type { get; set; }
    }
}