namespace System.Reflection
{
    internal static class AssemblyExtensions
    {
        public static string GetVersion(this Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetName().Version.ToString();
        }

        public static string GetInformationalVersion(this Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        }

        public static string GetFileVersion(this Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        }
    }

}
