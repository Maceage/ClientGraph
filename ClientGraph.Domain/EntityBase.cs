using System;

namespace ClientGraph.Domain
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }

        public abstract string GetDescription();
    }
}