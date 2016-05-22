using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ClientGraph.Domain.Enumerations;

namespace ClientGraph.Models
{
    public class RelationshipModel
    {
        [DisplayName("Parent Entity Type")]
        [Required]
        public EntityType? ParentEntityType { get; set; }

        [DisplayName("Parent Entity")]
        [Required]
        public Guid? ParentEntityId { get; set; }

        public string ParentEntityName { get; set; }

        [DisplayName("Child Entity Type")]
        [Required]
        public EntityType? ChildEntityType { get; set; }

        [DisplayName("Child Entity")]
        [Required]
        public Guid? ChildEntityId { get; set; }

        public string ChildEntityName { get; set; }

        [DisplayName("Relationship Type")]
        [Required]
        public RelationshipType RelationshipType { get; set; }
    }
}