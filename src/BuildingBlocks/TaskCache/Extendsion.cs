using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Dy.TaskCache
{
    public static class Extendsion
    {
        public static T ToObject<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }


        public static string ToJson<T>(this T o)
        {
            if (o == null)
                return string.Empty;
            return JsonConvert.SerializeObject(o);
        }
    }
}
