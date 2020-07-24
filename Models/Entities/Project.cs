using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Interfaces;
using Models.Relationships;

namespace Models.Entities
{
    public class Project : IEntityUserProp, IEntity
    {
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(256)")]
        public string Title { get; set; }

        [Column(TypeName="text")]
        public string Description { get; set; }
        
        public User User { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public List<UserVote> Votes { get; set; }
        
        public List<Comment> Comments { get; set; }
        
        public List<ProjectCategoryRelationship> ProjectCategoryRelationships { get; set; }
        
        [Column(TypeName = "varchar(256)")]
        public string Link { get; set; }
    }
}