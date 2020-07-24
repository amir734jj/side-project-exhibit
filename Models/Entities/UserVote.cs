using Models.Enums;
using Models.Interfaces;

namespace Models.Entities
{
    public class UserVote : IEntity
    {
        public int Id { get; set; }
        
        public Vote Value { get; set; }
        
        public Project Project { get; set; }
        
        public int ProjectId { get; set; }
        
        public User User { get; set; }
        
        public int UserId { get; set; }
    }
}