﻿//Mediator
namespace Assets.Scripts.Controllers.GameController
{
    using Commands;
    using Commands.Client;
    using EventArgs;
    using Exceptions;
    using Interfaces;
    using Network.NetworkManagers;
    using Notifications;
    using Utils.Unity;

    using UnityEngine;

    using EventArgs = System.EventArgs;

    public class BasicExamAndroidUIController : ExtendedMonoBehaviour
    {
        public GameObject QuestionPanelUI;
        public GameObject ConnectingUI;
        public GameObject EndGameUI;
        public GameObject LeaderboardUI;
        public GameObject UnableToConnectUI;
        public GameObject SecondsRemainingUI;
        public GameObject MainPlayerDialogUI;

        public ClientNetworkManager NetworkManager;
        public NotificationsServiceController NotificationsController;
        public SecondsRemainingUIController SecondsRemainingUIController;
        public DialogController MainPlayerDialogController;

        private QuestionUIController questionUIController = null;

        private UnableToConnectUIController unableToConnectUIController = null;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            this.LoadControllers();
            this.LoadCommands();
            this.AttachEventsHooks();
            this.ConnectToServer();
        }

        private void ConnectToServer()
        {
            var localIp = PlayerPrefsEncryptionUtils.GetString("ServerLocalIP");
            var externalIp = PlayerPrefsEncryptionUtils.HasKey("ServerExternalIP") ? PlayerPrefsEncryptionUtils.GetString("ServerExternalIP") : localIp;

            //TODO: use NetworkManager.GetServerIp()
            this.CoroutineUtils.WaitForFrames(1, () =>
                {
                    NetworkManagerUtils.Instance.IsServerUp(localIp, ClientNetworkManager.Port, (isRunning) =>
                        {
                            var serverIp = isRunning ? localIp : externalIp;
                            this.unableToConnectUIController.ServerIP = serverIp;
                            this.ConnectTo(serverIp);
                        });
                });
        }

        private void LoadControllers()
        {
            this.questionUIController = this.QuestionPanelUI.GetComponent<QuestionUIController>();
            this.unableToConnectUIController = this.UnableToConnectUI.GetComponent<UnableToConnectUIController>();
        }

        private void LoadCommands()
        {
            var answerTimeout = new AnswerTimeoutCommand(this.QuestionPanelUI, this.NotificationsController);
            var loadQuestion = new LoadQuestionCommand(this.LoadQuestion);
            var basicExamGameEnd = new BasicExamGameEndCommand(this.EndGameUI, this.LeaderboardUI);

            this.NetworkManager.CommandsManager.AddCommand(answerTimeout);
            this.NetworkManager.CommandsManager.AddCommand(loadQuestion);
            this.NetworkManager.CommandsManager.AddCommand(basicExamGameEnd);
        }

        private void AttachEventsHooks()
        {
            this.NetworkManager.OnConnectedEvent += this.OnConnected;
            this.NetworkManager.OnDisconnectedEvent += this.OnDisconnectFromServer;
            this.questionUIController.OnAnswerClick += this.OnAnswerClick;
        }

        private void OnAnswerClick(object sender, AnswerEventArgs args)
        {
            var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
            answerSelectedCommand.AddOption("Answer", args.Answer);
            this.NetworkManager.SendServerCommand(answerSelectedCommand);

            this.QuestionPanelUI.SetActive(false);
            this.SecondsRemainingUI.SetActive(false);
            //MainPlayerDialogUI.SetActive(true);
        }

        private void OnConnected(object sender, EventArgs args)
        {
            this.ConnectingUI.SetActive(false);
            this.Vibrate();
        }

        /// <summary>
        /// Vibrate if mobile.
        /// </summary>
        private void Vibrate()
        {
#if UNITY_ANDROID
        Handheld.Vibrate();
        #endif
        }

        private void OnDisconnectFromServer(object sender, EventArgs args)
        {
            this.ConnectingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(true);
        }

        private void LoadQuestion(ISimpleQuestion question, int timeToAnswer)
        {
            this.QuestionPanelUI.SetActive(true);
            this.questionUIController.LoadQuestion(question);

            this.SecondsRemainingUI.SetActive(true);
            this.SecondsRemainingUIController.SetSeconds(timeToAnswer);
            this.SecondsRemainingUIController.Paused = false;
        }

        public void ConnectTo(string ip)
        {
            this.ConnectingUI.SetActive(true);

            try
            {
                this.NetworkManager.ConnectToHost(ip);    
            }
            catch (NetworkException e)
            {
                var error = (NetworkConnectionError)e.ErrorN;
                var errorMessage = NetworkErrorUtils.GetMessage(error);
                this.NotificationsController.AddNotification(Color.red, errorMessage);
            }
        }

        public void Disconnect()
        {
            try
            {
                this.NetworkManager.Disconnect();
            }
            catch (NetworkException e)
            {
                var error = (NetworkConnectionError)e.ErrorN;
                var errorMessage = NetworkErrorUtils.GetMessage(error);
                this.NotificationsController.AddNotification(Color.red, errorMessage);
            }
        }
    }

}