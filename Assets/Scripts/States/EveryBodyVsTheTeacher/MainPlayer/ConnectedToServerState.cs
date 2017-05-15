using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using MainPlayerRequestedGameStartCommand = Commands.EveryBodyVsTheTeacher.PlayersConnectingState.MainPlayerRequestedGameStartCommand;
using NetworkCommandData = Commands.NetworkCommandData;
using StartGameRequestCommand = Commands.EveryBodyVsTheTeacher.StartGameRequestCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer
{

    using System;

    using Assets.Scripts.Interfaces;

    using StateMachine;

    using UnityEngine;
    using UnityEngine.UI;

    public class ConnectedToServerState : IState
    {
        private readonly IClientNetworkManager networkManager;

        private readonly Button gameStartButton;

        public ConnectedToServerState(IClientNetworkManager networkManager, Button gameStartButton)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameStartButton == null)
            {
                throw new ArgumentNullException("gameStartButton");
            }

            this.networkManager = networkManager;
            this.gameStartButton = gameStartButton;
            this.gameStartButton.onClick.AddListener(this.OnRequestedGameStart);
        }

        private void OnRequestedGameStart()
        {
            var requestStartGameCommand = NetworkCommandData.From<MainPlayerRequestedGameStartCommand>();
            this.networkManager.SendServerCommand(requestStartGameCommand);
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.gameStartButton.gameObject.SetActive(true);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.gameStartButton.gameObject.SetActive(false);
        }
    }
}