using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class RemainingTimeEventArgs : System.EventArgs
{
    public RemainingTimeEventArgs(int seconds)
    {
        if (seconds < 0)
        {
            throw new ArgumentOutOfRangeException("seconds");
        }

        this.Seconds = seconds;
    }

    public int Seconds
    {
        get;
        private set;
    }
}
