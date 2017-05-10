namespace Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter
{

    using System;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class ConsultWithTeacherJoker : IJoker
    {
        public Sprite Image { get; private set; }

        public event EventHandler OnActivated;

        public event EventHandler<UnhandledExceptionEventArgs> OnError;

        public event EventHandler OnFinishedExecution;

        public bool Activated { get; private set; }

        public void Activate()
        {
            throw new NotImplementedException();
        }
    }
}