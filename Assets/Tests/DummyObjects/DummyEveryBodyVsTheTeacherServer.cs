namespace Tests.DummyObjects
{

    using System;
    using System.Collections.Generic;

    using EventArgs;

    using Interfaces.Network;

    public class DummyEveryBodyVsTheTeacherServer : IEveryBodyVsTheTeacherServer
    {
        public event EventHandler OnGameOver = delegate
            {
            };

        public event EventHandler<ClientConnectionIdEventArgs> OnAddedMainPlayerToSurrenderList = delegate { };

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

        public IEnumerable<int> SurrenderedMainPlayersConnectionIds
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

        public void AddMainPlayerToSurrenderList(int connectionId)
        {
            this.OnAddedMainPlayerToSurrenderList(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void EndGame()
        {
            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);
        }
    }
}
