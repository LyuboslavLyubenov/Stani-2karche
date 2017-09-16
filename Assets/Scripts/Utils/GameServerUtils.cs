namespace Scripts.Utils
{
    using System.Diagnostics;

    public class GameServerUtils
    {
        public static Process StartServer(string gameTypeName)
        {
			#if UNITY_STANDALONE_LINUX
			var extension = ".x86";
			#else
			var extension = ".exe";
			#endif
			var serverPath = string.Format("Servers\\{0}\\server{1}", gameTypeName, extension);
            var processInfo = new ProcessStartInfo(serverPath, "-batchmode -nographics");
            var process = Process.Start(processInfo);
            return process;
        }
    }
}
