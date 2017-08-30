namespace Scripts.Utils
{
    public class GameServerUtils
    {
        public static void StartServer(string gameTypeName)
        {
            var serverPath = string.Format("Servers\\{0}\\server.exe", gameTypeName);
            System.Diagnostics.Process.Start(serverPath);
        }
    }
}
