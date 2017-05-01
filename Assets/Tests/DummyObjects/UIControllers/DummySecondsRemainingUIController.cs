namespace Assets.Tests.DummyObjects.UIControllers
{

    using System;

    using Assets.Scripts.Interfaces.Controllers;

    public class DummySecondsRemainingUIController : ISecondsRemainingUIController
    {
        public event EventHandler OnSecondPassed = delegate { };

        public event EventHandler OnFinished = delegate { };

        public int RemainingSecondsToAnswer
        {
            get; set;
        }

        public int InvervalInSeconds
        {
            get; set;
        }

        public bool Paused
        {
            get; set;
        }

        public bool Running
        {
            get; set;
        }

        public void StartTimer()
        {
            throw new NotImplementedException();
        }

        public void StopTimer()
        {
            throw new NotImplementedException();
        }


        public void FakeSecondPassed()
        {
            this.OnSecondPassed(this, EventArgs.Empty);
        }

        public void FakeFinished()
        {
            this.OnFinished(this, EventArgs.Empty);
        }
    }

}