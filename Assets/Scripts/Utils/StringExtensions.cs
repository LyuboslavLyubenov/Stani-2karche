using UnityEngine;
using System.Collections;
using System;

public static class StringExtensions
{
    public static bool IsValidIPV4(this string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress) || ipAddress.Split('.').Length != 4)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
