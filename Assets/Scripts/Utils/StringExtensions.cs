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

    public static bool ToBoolean(this string value)
    {
        switch (value.ToUpperInvariant())
        {
            case "TRUE":
                return true;
            case "T":
                return true;
            case "1":
                return true;
            case "0":
                return false;
            case "FALSE":
                return false;
            case "F":
                return false;
            default:
                throw new InvalidCastException("You can't cast a weird value to a bool!");
        }
    }
}
