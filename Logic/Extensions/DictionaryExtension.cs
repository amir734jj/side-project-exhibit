using System.Collections.Generic;

namespace Logic.Extensions
{
    public static class DictionaryExtension
    {
        public static void CopyTo(this IDictionary<string, object> source, IDictionary<string, object> destination, params string[] keys)
        {
            foreach (var key in keys)
            {
                destination[key] = source[key];
            }
            
            source.Clear();
        }

        public static object GetOrElse<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            return source.ContainsKey(key) && source[key] != null ? source[key] : defaultValue;
        } 
    }
}