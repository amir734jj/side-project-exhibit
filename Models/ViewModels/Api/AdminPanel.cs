using System.Collections.Generic;
using Models.Entities;

namespace Models.ViewModels.Api
{
    public class AdminPanel
    {
        public List<Project> Projects { get; set; }
        
        public List<Comment> Comments { get; set; }
        
        public List<Category> Categories { get; set; }
    }
}