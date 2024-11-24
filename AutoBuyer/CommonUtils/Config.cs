using System.Collections.Generic;
using System.Reflection;

namespace AutoBuyer.Common
{
    public interface IParser
    {
        Dictionary<string, object> Parse(string input);
    }

    public class Config: IConfig
    {
        private Dictionary<string, object> _dictionary;

        protected Config(Dictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }

        public static IConfig Build(IParser parser, Assembly assembly = null, string resource = "secrets.json")
        {
            if (assembly is null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            var json = ManifestReader.ReadManifestData(resource, assembly);
            var dict = parser.Parse(json);
            var config = new Config(dict);
            config.Connection = string.Format((string)config["format"], config["server"], config["username"], config["password"]);
            return config;
        }

        public string Connection
        {
            get => (string)this["connection"];
            set => this["connection"] = value;
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
            private set
            { 
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.Add(key, value);
                }
            }
        }
    }
}
