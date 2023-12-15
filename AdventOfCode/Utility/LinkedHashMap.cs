using System.Collections.Specialized;

namespace AdventOfCode.Utility
{
    public sealed class LinkedHashMap
        : NameValueCollection
    {
        public string Pop(string key)
        {
            var value = Get(key);

            Remove(key);

            return value;
        }
    }
}
