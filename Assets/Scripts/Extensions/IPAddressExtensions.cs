namespace Extensions
{

    using System.Linq;
    using System.Net;

    public static class IPAddressExtensions
    {
        public static string GetIPAddress(this IPAddress ipAddress)
        {
            return ipAddress.ToString().Split(':').First();
        }
    }

}
