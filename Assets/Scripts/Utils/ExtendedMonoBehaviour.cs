using UnityEngine;
using System;
using CielaSpike;
using System.Collections;

public abstract class ExtendedMonoBehaviour : MonoBehaviour
{
    CoroutineUtils coroutineUtils;

    public CoroutineUtils CoroutineUtils
    {
        get
        {
            if (coroutineUtils == null)
            {
                coroutineUtils = new CoroutineUtils(this);
            }

            return coroutineUtils;
        }
    }
}
