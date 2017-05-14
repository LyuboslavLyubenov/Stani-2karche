using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using PlayerPrefsEncryptionUtils = Utils.Unity.PlayerPrefsEncryptionUtils;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;
    
    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Usage;

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
        
        void Start()
        {
            PlayerPrefsEncryptionUtils.SetString("ServerExternalIP", "192.168.0.101");
            PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", "127.0.0.1");
            
            this.networkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
            
            this.stateMachine.SetCurrentState(this.notConnectedToServerState);
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
    }
}