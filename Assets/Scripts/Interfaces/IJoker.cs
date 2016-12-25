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

}