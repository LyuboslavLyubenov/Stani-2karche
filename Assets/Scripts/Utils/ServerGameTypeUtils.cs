namespace Assets.Scripts.Utils
{

    using System;
    using System.Linq;
    using System.Reflection;

    public class ServerGameTypeUtils
    {
        ServerGameTypeUtils()
        {
        }

        public static Type GetGameServerType(string gameTypeName)
        {
            var typeName = gameTypeName + "Server";
            return 
                Assembly.GetExecutingAssembly()
                .GetType(typeName);
        }
    }
}
