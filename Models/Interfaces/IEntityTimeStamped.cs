using System;

namespace Models.Interfaces
{
    public interface IEntityTimeStamped : IEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
    }
}