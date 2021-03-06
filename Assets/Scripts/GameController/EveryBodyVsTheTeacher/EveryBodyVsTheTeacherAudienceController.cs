﻿using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using PlayerPrefsEncryptionUtils = Utils.Unity.PlayerPrefsEncryptionUtils;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Audience;

    using StateMachine;

    using UnityEngine;

    using Zenject;

    public class EveryBodyVsTheTeacherAudienceController : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private NotConnectedToServerState notConnectedToServerState;

        [Inject]
        private ConnectedToServerState connectedToServerState;

        [Inject]
        private StateMachine stateMachine;

        void Start()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            PlayerPrefsEncryptionUtils.SetString("Username", "Test");
#endif
            this.networkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnectedToServer;

            this.stateMachine.SetCurrentState(this.notConnectedToServerState);
        }
        
        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.stateMachine.SetCurrentState(this.connectedToServerState);
        }

        private void OnDisconnectedToServer(object sender, EventArgs args)
        {
            this.stateMachine.SetCurrentState(this.notConnectedToServerState);
        }
    }
}