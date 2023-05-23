using System.Reflection;

namespace Michael.Net.Helpers
{
    public class PathHelper
    {
        public static string GetApplicationPath()
        {
            var currentPath = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(currentPath)!.Replace("file:\\", "");
        }
    }
}
