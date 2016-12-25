using System.Linq;
using System.Net;

namespace Assets.Scripts.Utils
{

    public static class IPAddressExtensions
    {
        public static string GetIPAddress(this IPAddress ipAddress)
        {
            return ipAddress.ToString().Split(':').First();
        }
    }

}
