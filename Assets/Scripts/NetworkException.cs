using UnityEngine;
using System.Collections;
using System;

public class NetworkException : Exception
{
    public NetworkException(byte errorN)
    {
        this.ErrorN = errorN;
    }

    public byte ErrorN
    {
        get;
        private set;
    }
}
