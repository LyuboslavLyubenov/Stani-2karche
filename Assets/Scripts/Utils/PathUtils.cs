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
            var assemblyPath = Path.GetDirectoryName(assembly.Location);
            return Directory.GetParent(assemblyPath).Parent.FullName;
        }
    }
}
