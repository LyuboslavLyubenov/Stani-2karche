using System.Net;
using System.Linq;

public static class IPAddressExtensions
{
    public static string GetIPAddress(this IPAddress ipAddress)
    {
        return ipAddress.ToString().Split(':').First();
    }
}
