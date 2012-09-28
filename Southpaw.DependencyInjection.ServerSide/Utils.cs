using System.Collections.Generic;

namespace Southpaw.DependencyInjection.ServerSide
{
    public static class Utils
    {
        public static Dictionary<string, string> ParseVariables(string vars)
        {
            var ret = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(vars))
                return ret;
            foreach(var kvp in vars.Split(','))
            {
                var kv = kvp.Split('=');
                ret[kv[0]] = kv[1];
            }
            return ret;
        }
    }
}