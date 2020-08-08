using System;
using System.Collections.Generic;
using System.Linq;
using Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Models
{
    public class IgnoreUserContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var allProperties = base.CreateProperties(type, memberSerialization);
            
            if (type == typeof(User))
            {
                var availableProperties = new[] {"id", "userName"};

                return allProperties.Where(x => availableProperties.Contains(x.PropertyName)).ToList();
            }

            return allProperties;
        }
    }
}