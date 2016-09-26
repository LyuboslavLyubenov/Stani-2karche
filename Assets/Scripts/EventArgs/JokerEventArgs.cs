using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Timers;
using System.Collections;

public class JokerEventArgs : EventArgs
{
    public Type JokerType
    {
        get;
        private set;
    }

    public JokerEventArgs(Type jokerType)
    {
        JokerUtils.ValidateJokerType(jokerType);
        this.JokerType = jokerType;
    }
}