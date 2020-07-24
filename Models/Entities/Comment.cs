using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Interfaces;

namespace Models.Entities
{
    public class Comment : IEntityTimeStamped, IEntityUserProp
    {
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(256)")]
        public string Text { get; set; }
        
        public User User { get; set; }
        
        public Project Idea { get; set; }
        
        public DateTimeOffset CreatedOn { get; set; }
    }
}