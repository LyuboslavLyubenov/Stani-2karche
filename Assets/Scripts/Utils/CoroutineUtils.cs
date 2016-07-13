﻿using UnityEngine;
using System.Collections;
using System;

public class CoroutineUtils
{
    MonoBehaviour instance;

    public CoroutineUtils(MonoBehaviour instance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException("instance");
        }

        this.instance = instance;
    }

    public void WaitForSeconds(float seconds, Action callback)
    {
        instance.StartCoroutine(WaitForSecondsCoroutine(seconds, callback));
    }

    public void WaitForFrames(int frames, Action callback)
    {
        instance.StartCoroutine(WaitForSecondsCoroutine(frames, callback));
    }

    public void WaitForRenderGUIFrames(int frames, Action callback)
    {
        instance.StartCoroutine(WaitForGUIRenderFrameCoroutine(frames, callback));
    }

    public void WaitUntil(Func<bool> condition, Action callback)
    {
        instance.StartCoroutine(WaitUntilCoroutine(condition, callback));
    }

    IEnumerator WaitUntilCoroutine(Func<bool> condition, Action callback)
    {
        yield return new WaitUntil(condition);
        callback();
    }

    IEnumerator WaitForGUIRenderFrameCoroutine(int frames, Action callback)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForEndOfFrame();    
        }

        callback();
    }

    IEnumerator WaitForFramesCoroutine(int frames, Action callback)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;    
        }

        callback();
    }

    static IEnumerator WaitForSecondsCoroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }
}

