using AutoBuyer.Common;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Tests.Utils
{
    public sealed class EmbeddedResourceDataAttribute : DataAttribute
    {
        private readonly string[] _args;

        public EmbeddedResourceDataAttribute(params string[] args)
        {
            _args = args;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var result = new object[_args.Length];
            for (var index = 0; index < _args.Length; index++)
            {
                result[index] = ManifestReader.ReadManifestData(_args[index]);
            }
            return new[] { result };
        }
    }
}
