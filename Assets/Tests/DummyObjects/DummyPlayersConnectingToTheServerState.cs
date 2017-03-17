namespace Assets.Tests.DummyObjects
{

    using System;
    using System.Collections.ObjectModel;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Assets.Scripts.StateMachine;

    public class DummyPlayersConnectingToTheServerState : IPlayersConnectingToTheServerState
    {
        public event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerConnected = delegate { };

        public event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerDisconnected = delegate { };

        public event EventHandler OnEveryBodyRequestedGameStart = delegate { };

        public event EventHandler<ClientConnectionDataEventArgs> OnMainPlayerRequestedGameStart = delegate { };

        public event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerConnected = delegate { };

        public event EventHandler<ClientConnectionDataEventArgs> OnAudiencePlayerDisconnected = delegate { };

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
            this.OnMainPlayerConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void SimulateMainPlayerDisconnected(int connectionId)
        {
            this.OnMainPlayerDisconnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void SimulateAudiencePlayerConnected(int connectionId)
        {
            this.OnAudiencePlayerConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void SimulateAudiencePlayerDisconnected(int connectionId)
        {
            this.OnAudiencePlayerDisconnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void SimulateMainPlayerRequestedGameStart(int connectionId)
        {
            this.OnMainPlayerRequestedGameStart(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void SimulateAllMainPlayersRequestedGameStart()
        {
            this.OnEveryBodyRequestedGameStart(this, EventArgs.Empty);
        }
    }
}
