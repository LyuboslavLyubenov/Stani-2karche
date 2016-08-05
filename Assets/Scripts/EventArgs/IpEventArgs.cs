﻿using System;
using System.Net;

public class IpEventArgs : EventArgs
{
    public IpEventArgs(string ip)
    {
        if (!ip.IsValidIPV4())
        {
            throw new ArgumentException("Invalid ipv4 address");  
        }

        this.IPAddress = ip;
    }

    public string IPAddress
    {
        get;
        private set;
    }
}