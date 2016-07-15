using System;

public class IpEventArgs : EventArgs
{
    public IpEventArgs(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            throw new ArgumentNullException(ip);
        }

        if (ip.Split('.').Length != 4)
        {
            throw new ArgumentException("Invalid ip address");
        }

        this.IPAddress = ip;
    }

    public string IPAddress
    {
        get;
        set;
    }
}