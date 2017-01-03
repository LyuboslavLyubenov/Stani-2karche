using System;

using UnityEngine;

namespace Assets.Scripts.Interfaces
{

    public interface IJoker
    {
        Sprite Image
        {
            get;
        }

        event EventHandler OnActivated;

        event EventHandler<UnhandledExceptionEventArgs> OnError;

        event EventHandler OnFinishedExecution;

        bool Activated
        {
            get;
        }

        void Activate();
    }

}