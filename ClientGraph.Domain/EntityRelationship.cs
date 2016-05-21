﻿using System;
using ClientGraph.Domain.Enumerations;

namespace ClientGraph.Domain
{
    public class EntityRelationship
    {
        public EntityType ParentEntityType { get; set; }
        public Guid ParentEntityId { get; set; }

        public EntityType ChildEntityType { get; set; }
        public Guid ChildEntityId { get; set; }
    }
}