using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api.Extensions
{
    public static class ObjectExtension
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        public static string ToJson<T>(this T source)
        {
            return JsonConvert.SerializeObject(source, SerializerSettings);
        }
    }
}