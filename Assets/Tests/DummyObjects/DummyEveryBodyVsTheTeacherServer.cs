namespace Tests.DummyObjects
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network;

    public class DummyEveryBodyVsTheTeacherServer : IEveryBodyVsTheTeacherServer
    {
        public event EventHandler OnGameOver = delegate
            {
            };

        public bool IsGameOver
        {
            get; set;
        }

        public IEnumerable<int> ConnectedMainPlayersConnectionIds
        {
            get; set;
        }

        public IEnumerable<int> MainPlayersConnectionIds
        {
            get; set;
        }

        public bool StartedGame
        {
            get; set;
        }

        public int PresenterId
        {
            get; set;
        }

        public void EndGame()
        {
            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);
        }
    }
}
