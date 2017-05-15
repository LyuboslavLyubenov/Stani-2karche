namespace GameController.EveryBodyVsTheTeacher
{

    using System;

    using Commands;
    using Commands.Server;

    using Controllers;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using States.EveryBodyVsTheTeacher.Shared;

    using UnityEngine;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class EverybodyVsTheTeacherMainPlayerController : ExtendedMonoBehaviour
    {
        public GameObject LoadingUI;
        public UnableToConnectUIController UnableToConnectUIController;

        [Inject]
        private StateMachine stateMachine;
        
        [Inject]
        private NotConnectedToServerState notConnectedToServerState;

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