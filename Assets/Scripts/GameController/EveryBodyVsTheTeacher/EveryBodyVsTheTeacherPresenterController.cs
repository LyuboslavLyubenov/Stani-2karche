using AudiencePlayersContainerUIController = Controllers.EveryBodyVsTheTeacher.PlayersConnecting.AudiencePlayersContainerUIController;
using ElectionQuestionUIController = Controllers.ElectionQuestionUIController;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using MainPlayersContainerUIController = Controllers.EveryBodyVsTheTeacher.PlayersConnecting.MainPlayersContainerUIController;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using PlayersConnectingState = States.EveryBodyVsTheTeacher.Presenter.PlayersConnectingState;
using UnableToConnectUIController = Controllers.UnableToConnectUIController;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{

    using System;

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;
    
    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class EveryBodyVsTheTeacherPresenterController : MonoBehaviour
    {
        [SerializeField]
        private GameObject LoadingUI;

        [SerializeField]
        private MainPlayersContainerUIController MainPlayersContainerUIController;

        [SerializeField]
        private AudiencePlayersContainerUIController AudiencePlayersContainerUIController;

        [SerializeField]
        private UnableToConnectUIController UnableToConnectUIController;

        [SerializeField]
        private ElectionQuestionUIController ElectionQuestionUIController;

        [Inject]
        private IClientNetworkManager clientNetworkManager;
        
        private PlayersConnectingState playersConnectingState;
        private NotConnectedToServerState notConnectedToServerState;
        private PlayingState playingState;

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
            this.stateMachine.SetCurrentState(this.playingState);
        }
    }
}