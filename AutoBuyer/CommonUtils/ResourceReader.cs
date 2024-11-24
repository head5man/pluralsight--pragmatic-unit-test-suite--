using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AutoBuyer.Common
{
    public static class ManifestReader
    {
        public static string ReadManifestData(string resourceName, Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            var resource = assembly.GetManifestResourceNames().FirstOrDefault(name => name == resourceName.Replace("/", "."));
            if (resource == null)
            {
                var resourceFileName = resourceName.Split('/').LastOrDefault();
                var resources = assembly.GetManifestResourceNames();
                resource = resources.FirstOrDefault(r => r.EndsWith(resourceFileName));
            }

            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
