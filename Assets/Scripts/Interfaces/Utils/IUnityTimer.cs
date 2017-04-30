namespace Assets.Scripts.Interfaces.Utils
{

    using System;

    public interface IUnityTimer
    {
        event EventHandler OnSecondPassed;
        event EventHandler OnFinished;

        int InvervalInSeconds
        {
            get; set;
        }
        
        bool Paused
        {
            get; set;
        }

        bool Running
        {
            get;
        }

        void StartTimer();

        void StopTimer();
    }
}
