using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using PlayerPrefsEncryptionUtils = Utils.Unity.PlayerPrefsEncryptionUtils;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;
    
    using StateMachine;

    using UnityEngine;

    using Zenject;

    public class EveryBodyVsTheTeacherPresenterController : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private PlayersConnectingState playersConnectingState;

        [Inject]
        private NotConnectedToServerState notConnectedToServerState;

        [Inject]
        private PlayingState playingState;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private IRemoteStateActivator remoteStateActivator;
        
        void Start()
        {
            PlayerPrefsEncryptionUtils.SetString("Username", "Ivan");
            PlayerPrefsEncryptionUtils.SetString("ServerExternalIP", "127.0.0.1");
            PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", "127.0.0.1");
            
            this.networkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
            
            this.stateMachine.SetCurrentState(this.notConnectedToServerState);

            this.remoteStateActivator.Bind("PlayersConnectingState", this.playersConnectingState);
            this.remoteStateActivator.Bind("PlayingState", this.playingState);
        }

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            if (this.stateMachine.CurrentState == this.playersConnectingState)
            {
                return;
            }

            var presenterConnectingcommand = NetworkCommandData.From<PresenterConnectingCommand>();
            this.networkManager.SendServerCommand(presenterConnectingcommand);

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
    }
}