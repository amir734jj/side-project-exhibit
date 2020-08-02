using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Models.Entities;
using Newtonsoft.Json;

namespace Models.ViewModels.Api
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public List<string> Categories { get; set; } = new List<string>();
        
        public List<UserVote> Votes { get; set; } = new List<UserVote>();
    }
}