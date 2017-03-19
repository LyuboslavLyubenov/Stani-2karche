namespace Tests.DummyObjects
{

    using System;
    using System.Collections.ObjectModel;

    using EventArgs;

    using Interfaces.Network.EveryBodyVsTheTeacher.States;

    using StateMachine;

    public class DummyPlayersConnectingToTheServerState : IPlayersConnectingToTheServerState
    {
        public event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerConnected = delegate { };

        public event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerDisconnected = delegate { };

        public event EventHandler OnEveryBodyRequestedGameStart = delegate { };

        public event EventHandler<ClientConnectionIdEventArgs> OnMainPlayerRequestedGameStart = delegate { };

        public event EventHandler<ClientConnectionIdEventArgs> OnAudiencePlayerConnected = delegate { };

        public event EventHandler<ClientConnectionIdEventArgs> OnAudiencePlayerDisconnected = delegate { };

        public ReadOnlyCollection<int> MainPlayersConnectionIds
        {
            get; set;
        }

        public ReadOnlyCollection<int> AudiencePlayersConnectionIds
        {
            get; set;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            //
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            //
        }

        public void SimulateMainPlayerConnected(int connectionId)
        {
            this.OnMainPlayerConnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void SimulateMainPlayerDisconnected(int connectionId)
        {
            this.OnMainPlayerDisconnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void SimulateAudiencePlayerConnected(int connectionId)
        {
            this.OnAudiencePlayerConnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void SimulateAudiencePlayerDisconnected(int connectionId)
        {
            this.OnAudiencePlayerDisconnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void SimulateMainPlayerRequestedGameStart(int connectionId)
        {
            this.OnMainPlayerRequestedGameStart(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void SimulateAllMainPlayersRequestedGameStart()
        {
            this.OnEveryBodyRequestedGameStart(this, EventArgs.Empty);
        }
    }
}
