using System;
using System.Collections.Generic;

namespace ClientGraph.Domain
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }

        public string Name => GetDescription();

        public IList<EntityVersion> Versions { get; set; }

        protected abstract string GetDescription();
    }
}