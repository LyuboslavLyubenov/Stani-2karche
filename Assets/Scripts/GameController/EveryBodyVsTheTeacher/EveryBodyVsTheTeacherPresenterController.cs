using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using PlayersConnectingState = States.EveryBodyVsTheTeacher.Presenter.PlayersConnectingState;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using System;

    using Controllers;
    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;
    
    using StateMachine;

    using States.EveryBodyVsTheTeacher.Shared;

    using UnityEngine;
    
    using Zenject.Source.Usage;

    public class EveryBodyVsTheTeacherPresenterController : MonoBehaviour
    {
        public GameObject LoadingUI;

        public MainPlayersContainerUIController MainPlayersContainerUIController;
        public AudiencePlayersContainerUIController AudiencePlayersContainerUIController;
        public UnableToConnectUIController UnableToConnectUIController;
        
        [Inject]
        private IClientNetworkManager clientNetworkManager;

        private PlayersConnectingState playersConnectingState;
        private NotConnectedToServerState notConnectedToServerState;

        private readonly StateMachine stateMachine = new StateMachine();
        
        void Start()
        {
            this.playersConnectingState = new PlayersConnectingState(
                this.MainPlayersContainerUIController, 
                this.AudiencePlayersContainerUIController, 
                this.clientNetworkManager,
                this.OnEveryBodyRequestedGameStart);

            this.notConnectedToServerState = 
                new NotConnectedToServerState(
                    this.LoadingUI, 
                    this.UnableToConnectUIController, 
                    this.clientNetworkManager);

            this.clientNetworkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.clientNetworkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }
        private void OnConnectedToServer(object sender, EventArgs args)
        {
            if (this.stateMachine.CurrentState == this.playersConnectingState)
            {
                return;
            }

            this.stateMachine.SetCurrentState(this.playersConnectingState);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            if (this.stateMachine.CurrentState == this.notConnectedToServerState)
            {
                return;
            }

            this.stateMachine.SetCurrentState(this.notConnectedToServerState);
        }
        
        private void OnEveryBodyRequestedGameStart()
        {
            //TODO: Change state
        }
    }
}