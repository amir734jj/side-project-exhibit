using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Interfaces;
using Models.Relationships;

namespace Models.Entities
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(256)")]
        public string Name { get; set; }
        
        public List<ProjectCategoryRelationship> ProjectCategoryRelationships { get; set; } = new List<ProjectCategoryRelationship>();
    }
}