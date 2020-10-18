using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities
{
    public class User : IdentityUser<int>, IEntity
    {
        public List<Project> Projects { get; set; }
        
        public string Name { get; set; }

        [Column(TypeName="text")]
        public string Description { get; set; }

        public List<Comment> Comments { get; set; }
        
        public DateTimeOffset LastLoginTime { get; set; }
        
        public List<UserVote> Votes { get; set; }
        
        [Display(Name = "User Role")]
        public UserRoleEnum UserRole { get; set; }
        
        [Column(TypeName = "jsonb")]
        public List<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        
        public object Obfuscate()
        {
            const string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

            var obfuscatedEmail = Regex.Replace(Email, pattern, m => new string('*', m.Length));
            
            return new {Email = obfuscatedEmail, Name};
        }
        
        public object ToAnonymousObject()
        {
            return new {Email, Name};
        }
    }
}