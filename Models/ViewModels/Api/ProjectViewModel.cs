using System.Collections.Generic;

namespace Models.ViewModels.Api
{
    public class ProjectViewModel
    {
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public List<string> Categories { get; set; }
    }
}