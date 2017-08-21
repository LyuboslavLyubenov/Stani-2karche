namespace Assets.Scripts.Utils
{
    using System.IO;
    using System.Reflection;

    public class PathUtils
    {
        PathUtils()
        {    
        }

        public static string GetGameDirectoryPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location) + "\\..\\..\\";
        }
    }
}
