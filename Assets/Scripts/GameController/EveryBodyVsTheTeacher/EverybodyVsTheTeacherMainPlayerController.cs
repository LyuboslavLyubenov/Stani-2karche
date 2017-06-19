namespace GameController.EveryBodyVsTheTeacher
{
    using System;
    
    using Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer;

    using Commands;
    using Commands.Server;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using States.EveryBodyVsTheTeacher.Shared;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class EverybodyVsTheTeacherMainPlayerController : ExtendedMonoBehaviour
    {
        [Inject]
        private StateMachine stateMachine;
        
        [Inject]
        private NotConnectedToServerState notConnectedToServerState;

        [Inject]
        private ConnectedToServerState connectedToServerState;

        [Inject]
        private IClientNetworkManager networkManager;

        void Start()
        {
            this.networkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
            
            this.stateMachine.SetCurrentState(this.notConnectedToServerState);
        }

        private void OnConnectedToServer(object sender, EventArgs eventArgs)
        {
            var mainPlayerConnectingCommand = NetworkCommandData.From<MainPlayerConnectingCommand>();
            this.networkManager.SendServerCommand(mainPlayerConnectingCommand);

            this.stateMachine.SetCurrentState(this.connectedToServerState);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs e)
        {
            if (this.stateMachine.CurrentState == this.notConnectedToServerState)
            {
                return;
            }

            this.stateMachine.SetCurrentState(this.notConnectedToServerState);
        }
    }
}