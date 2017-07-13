namespace Jokers
{

    using System;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class ConsultWithTheTeacherJoker : IJoker
    {
        public Sprite Image { get; private set; }

        public event EventHandler OnActivated;

        public event EventHandler<UnhandledExceptionEventArgs> OnError;

        public event EventHandler OnFinishedExecution;

        public bool Activated { get; private set; }

        public ConsultWithTheTeacherJoker()
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }
    }

}