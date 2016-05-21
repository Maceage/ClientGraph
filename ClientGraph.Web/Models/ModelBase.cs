using System;

namespace ClientGraph.Models
{
    public abstract class ModelBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}