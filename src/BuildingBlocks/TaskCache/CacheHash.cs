using System;
using System.Collections.Generic;
using System.Text;

namespace Dy.TaskCache
{
    public class CacheHash
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
    public static class CacheHashExtension
    {
        public static bool IsNull(this CacheHash cache)
        {
            if (cache == null || cache.Equals(new CacheHash()) || cache == new CacheHash())
            {
                return true;
            }
            return false;
        }
    }
}
