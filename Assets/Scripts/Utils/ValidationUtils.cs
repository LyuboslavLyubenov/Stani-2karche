using UnityEngine;
using System.Net;
using System;
using System.Collections;
using CielaSpike;

public class ValidationUtils
{
    ValidationUtils()
    {
        
    }

    public static void ValidateStringNotNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(paramName + " cannot be empty");
        }
    }

    public static void ValidateObjectNotNull(object obj, string paramName)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(paramName + " cannot be null");
        }
    }
}