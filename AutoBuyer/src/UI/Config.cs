using MySqlX.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Converters;
using AutoBuyer.Logic.Common;

namespace AutoBuyer.UI
{
    public class Config : IConfig
    {
        private Dictionary<string, object> _dictionary;

        private Config()
        {
            using (var jsonstream = File.OpenText("secrets.json"))
            {
                _dictionary = JsonParser.Parse(jsonstream.ReadToEnd());
            }
        }

        public static IConfig Build()
        {
            var config = new Config();
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
