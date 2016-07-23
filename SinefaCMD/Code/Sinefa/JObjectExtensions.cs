using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SinefaCMD.Sinefa
{
    static class JObjectExtensions
    {
        public static IEnumerable<JToken> Descendants(this JObject obj, string name)
        {
            return obj.Descendants().Where(t => t.Type == JTokenType.Property && ((JProperty) t).Name == name);
        }
    }
}