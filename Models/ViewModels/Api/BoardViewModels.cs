using System.Collections.Generic;
using Models.Entities;

namespace Models.ViewModels.Api
{
    public class BoardViewModels
    {
        public List<Project> Projects { get; set; }
        
        public int Pages { get; set; }
    }
}