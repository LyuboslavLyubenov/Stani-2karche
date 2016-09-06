using UnityEngine;
using System;

public interface IJoker
{
    Sprite Image
    {
        get;
    }

    EventHandler OnSuccessfullyActivated
    {
        get;
        set;
    }

    void Activate();
}