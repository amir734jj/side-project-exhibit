using System;

namespace Models.Entities
{
    public class UserNotification
    {
        public string Subject { get; set; }
        
        public string Text { get; set; }
        
        public bool Collected { get; set; }
        
        public DateTimeOffset DateTime { get; set; }
    }
}