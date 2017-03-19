using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IState = Interfaces.IState;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Shared
{
    using System;

    using Assets.Scripts.Interfaces.Controllers;

    using Commands;
    using Commands.Server;

    using Controllers;

    using Extensions;

    using Localization;

    using Notifications;

    using StateMachine;

    using UnityEngine;

    using Utils;
    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class NotConnectedToServerState : IState
    {
        private GameObject loadingUI;
        private GameObject unableToConnectToUI;

        private IUnableToConnectUIController unableToConnectUIController;

        private IClientNetworkManager networkManager;
        
        private Timer_ExecuteMethodAfterTime notFoundServerIPTimer;

        public NotConnectedToServerState(
            GameObject loadingUI,
            IUnableToConnectUIController unableToConnectUIController,
            IClientNetworkManager networkManager)
        {
            if (loadingUI == null)
            {
                throw new ArgumentNullException("loadingUI");
            }

            if (unableToConnectUIController == null)
            {
                throw new ArgumentNullException("unableToConnectUIController");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.loadingUI = loadingUI;
            this.unableToConnectUIController = unableToConnectUIController;
            this.networkManager = networkManager;

            this.unableToConnectToUI = ((UnableToConnectUIController)this.unableToConnectUIController).gameObject;
        }

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.unableToConnectUIController.gameObject.SetActive(false);

            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            this.networkManager.SendServerCommand(commandData);

            var connectedMsg = LanguagesManager.Instance.GetValue("EveryBodyVsTheTeacher/ConnectedToServer");
            NotificationsController.Instance.AddNotification(Color.blue, connectedMsg);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.unableToConnectToUI.SetActive(true);
        }

        private void OnFoundServerIP(string ip)
        {
            this.unableToConnectUIController.ServerIP = ip;
            this.networkManager.ConnectToHost(ip);
        }

        private void OnFoundServerIPError()
        {
            this.notFoundServerIPTimer.Reset();
        }

        private void GetServerIpAndConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        private void AttachEventHandlers()
        {
            this.networkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }

        private void DetachEventHandlers()
        {
            this.networkManager.OnConnectedEvent -= this.OnConnectedToServer;
            this.networkManager.OnDisconnectedEvent -= this.OnDisconnectedFromServer;
        }

        private void ConfigureNotFoundServerIPTimer()
        {
            this.notFoundServerIPTimer = TimerUtils.ExecuteAfter(1f, this.GetServerIpAndConnectToServer);
            this.notFoundServerIPTimer.AutoDispose = false;
            this.notFoundServerIPTimer.RunOnUnityThread = true;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.loadingUI.SetActive(true);
            this.unableToConnectToUI.SetActive(false);

            this.ConfigureNotFoundServerIPTimer();
            this.AttachEventHandlers();
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.DetachEventHandlers();

            this.notFoundServerIPTimer.Stop();
            this.notFoundServerIPTimer.Dispose();
            this.notFoundServerIPTimer = null;
        }
    }
}