namespace Scripts.Utils
{

    public class GameServerUtils
    {
        public static void StartServer(string gameTypeName)
        {
            var serverPath = string.Format("Server\\{0}.exe", gameTypeName);
            System.Diagnostics.Process.Start(serverPath);
        }
    }
}
