using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using PlayersConnectingState = Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter.PlayersConnectingState;

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
            this.stateMachine.SetCurrentState(this.notConnectedToServerState);

            this.networkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
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