using System;
using System.IO;

namespace UnityTestTools.IntegrationTestsFramework.TestRunner
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IntegrationTestAttribute : Attribute
    {
        private readonly string m_Path;

        public IntegrationTestAttribute(string path)
        {
            if (path.EndsWith(".unity"))
                path = path.Substring(0, path.Length - ".unity".Length);
            this.m_Path = path;
        }

        public bool IncludeOnScene(string scenePath)
        {
            if (scenePath == this.m_Path) return true;
            var fileName = Path.GetFileNameWithoutExtension(scenePath);
            return fileName == this.m_Path;
        }
    }

}
