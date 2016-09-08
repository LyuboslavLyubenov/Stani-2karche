using UnityEngine;
using System;

public interface IJoker
{
    Sprite Image
    {
        get;
    }

    EventHandler OnActivated
    {
        get;
        set;
    }

    bool Activated
    {
        get;
    }

    void Activate();
}