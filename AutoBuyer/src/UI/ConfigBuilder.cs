using AutoBuyer.Common;
using MySqlX.Serialization;

using System.Collections.Generic;
using System.Reflection;

namespace AutoBuyer.UI
{
    public  class ConfigBuilder
    {
        public static IConfig Build(string resource = "secrets.json")
        {
            return Config.Build(new Parser(), Assembly.GetAssembly(typeof(ConfigBuilder)), resource);
        }

        private class Parser : IParser
        {
            public Dictionary<string, object> Parse(string input)
            {
                return JsonParser.Parse(input);
            }
        }
    }
}
