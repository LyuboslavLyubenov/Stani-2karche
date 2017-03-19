using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using System;
    using System.Linq;

    using Controllers;
    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

    using Notifications;

    using StateMachine;
    using States.EveryBodyVsTheTeacher.Presenter;
    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class EveryBodyVsTheTeacherPresenterController : MonoBehaviour
    {
        private readonly StateMachine stateMachine = new StateMachine();

        public MainPlayersContainerUIController MainPlayersContainerUIController;
        public AudiencePlayersContainerUIController AudiencePlayersContainerUIController;
        public UnableToConnectUIController UnableToConnectUIController;

        private PlayersConnectingState playersConnectingState;

        [Inject]
        private IClientNetworkManager clientNetworkManager;

        void Start()
        {
            this.playersConnectingState = new PlayersConnectingState(
                this.MainPlayersContainerUIController, 
                this.AudiencePlayersContainerUIController, 
                this.clientNetworkManager,
                this.OnEveryBodyRequestedGameStart);

            this.clientNetworkManager.OnConnectedEvent += OnConnectedToServer;
            this.clientNetworkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;

            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        private void OnFoundServerIP(string ip)
        {
            this.UnableToConnectUIController.ServerIP = ip;
            this.clientNetworkManager.ConnectToHost(ip);
        }

        private void OnFoundServerIPError()
        {
            //TODO:
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
            //TODO:
            this.UnableToConnectUIController.gameObject.SetActive(true);
        }

        private void OnEveryBodyRequestedGameStart()
        {
            //TODO: Change state
        }
    }
}