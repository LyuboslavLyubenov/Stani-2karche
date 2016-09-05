﻿using System;
using UnityEngine;

public class DebugUtils
{
    public static void LogException(Exception e)
    {
        Debug.LogErrorFormat("Error {0} of type {1} stacktrace {2}", e.Message, e.GetType().Name, e.StackTrace);
    }
}
