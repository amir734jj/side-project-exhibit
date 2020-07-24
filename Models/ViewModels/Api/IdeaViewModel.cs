using System.Collections.Generic;

namespace Models.ViewModels.Api
{
    public class IdeaViewModel
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }
        
        public List<string> Categories { get; set; }
    }
}