using System.Reflection;
using Microsoft.DotNet.PlatformAbstractions;

namespace Api.Utilities
{
    public static class AssemblyInfo
    {
        public static readonly string AssemblyVersion;

        static AssemblyInfo()
        {
            AssemblyVersion = typeof(RuntimeEnvironment).GetTypeInfo()
                .Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        }
    }
}