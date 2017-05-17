using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace States.EveryBodyVsTheTeacher.Shared
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces;

    using Extensions;

    using Interfaces.Controllers;

    using Localization;

    using Notifications;

    using Scripts.Utils;

    using StateMachine;

    using UnityEngine;

    using Utils;
    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class NotConnectedToServerState : IState
    {
        private readonly GameObject loadingUI;
        private readonly GameObject unableToConnectUI;
        private readonly IUnableToConnectUIController unableToConnectUIController;
        private readonly IClientNetworkManager networkManager;
        private Timer_ExecuteMethodAfterTime notFoundServerIPTimer;
        private readonly NetworkCommandData connectingCommand = null;

        public NotConnectedToServerState(
            GameObject loadingUI,
            GameObject unableToConnectUI,
            IUnableToConnectUIController unableToConnectUIController,
            IClientNetworkManager networkManager,
            ClientType clientType)
        {
            if (loadingUI == null)
            {
                throw new ArgumentNullException("loadingUI");
            }

            if (unableToConnectUI == null)
            {
                throw new ArgumentNullException("unableToConnectUI");
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
            this.unableToConnectUI = unableToConnectUI;
            this.unableToConnectUIController = unableToConnectUIController;
            this.networkManager = networkManager;

            if (clientType == ClientType.MainPlayer)
            {
                this.connectingCommand = NetworkCommandData.From<MainPlayerConnectingCommand>();
            }
            else if (clientType == ClientType.Presenter)
            {
                this.connectingCommand = NetworkCommandData.From<PresenterConnectingCommand>();
            }
        }

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.unableToConnectUI.gameObject.SetActive(false);

            if (this.connectingCommand != null)
            {
                this.networkManager.SendServerCommand(this.connectingCommand);
            }
            
            var connectedMsg = LanguagesManager.Instance.GetValue("EveryBodyVsTheTeacher/ConnectedToServer");
            NotificationsController.Instance.AddNotification(Color.blue, connectedMsg);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.unableToConnectUI.SetActive(true);
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
            this.notFoundServerIPTimer = TimerUtils.ExecuteAfter(3f, this.GetServerIpAndConnectToServer);
            this.notFoundServerIPTimer.AutoDispose = false;
            this.notFoundServerIPTimer.RunOnUnityThread = true;
        }
        
        private void FindServerIpAndConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.loadingUI.SetActive(true);
            this.unableToConnectUI.SetActive(false);

            this.ConfigureNotFoundServerIPTimer();
            this.AttachEventHandlers();

            if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
            {
                PlayerPrefsEncryptionUtils.DeleteKey("MainPlayerHost");

                //wait until server is loaded. starting the server takes about ~7 seconds on i7 + SSD.
                var timer = TimerUtils.ExecuteAfter(9f, () => this.networkManager.ConnectToHost("127.0.0.1"));
                timer.AutoDispose = true;
                timer.RunOnUnityThread = true;

                GameServerUtils.StartServer("EveryBodyVsTheTeacher");
            }
            else
            {
                this.FindServerIpAndConnectToServer();
            }
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.DetachEventHandlers();

            this.notFoundServerIPTimer.Stop();
            this.notFoundServerIPTimer.Dispose();
            this.notFoundServerIPTimer = null;
        }
    }

    public enum ClientType
    {
        MainPlayer,
        Presenter,
        Audience
    }
}