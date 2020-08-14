using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace Models.ViewModels.Api
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [MinLength(5)]
        public string Title { get; set; }
        
        [Required]
        [MinLength(20)]
        public string Description { get; set; }
        
        [Required]
        public List<string> Categories { get; set; } = new List<string>();
        
        public List<UserVote> Votes { get; set; } = new List<UserVote>();
        
        public DateTimeOffset CreatedOn { get; set; }
    }
}