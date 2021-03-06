﻿//Mediator
// ReSharper disable ArrangeTypeMemberModifiers
namespace GameController
{

    using Commands;
    using Commands.Client;

    using Controllers;

    using EventArgs;

    using Interfaces;

    using Network.Leaderboard;
    using Network.NetworkManagers;

    using Notifications;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Utils.Unity;

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

        public SecondsRemainingUIController SecondsRemainingUIController;
        public DialogController MainPlayerDialogController;

        private QuestionUIController questionUIController = null;

        private UnableToConnectUIController unableToConnectUIController = null;

        private LeaderboardReceiver leaderboardReceiver;

        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            this.leaderboardReceiver = new LeaderboardReceiver(ClientNetworkManager.Instance, 10);

            this.LoadControllers();
            this.LoadCommands();
            this.AttachEventsHooks();
            this.ConnectToServer();
        }

        void OnApplicationQuit()
        {
            ClientNetworkManager.Instance.Dispose();

            this.leaderboardReceiver.Dispose();
            this.leaderboardReceiver = null;
        }

        private void ConnectToServer()
        {
            var localIp = PlayerPrefsEncryptionUtils.GetString("ServerLocalIP");
            var externalIp = PlayerPrefsEncryptionUtils.HasKey("ServerExternalIP") ? PlayerPrefsEncryptionUtils.GetString("ServerExternalIP") : localIp;

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
            var answerTimeout = new AnswerTimeoutCommand(this.QuestionPanelUI, NotificationsController.Instance);
            var loadQuestion = new LoadQuestionCommand(this.LoadQuestion);
            var basicExamGameEnd = new GameEndCommand(this.EndGameUI, this.LeaderboardUI, this.leaderboardReceiver);

            ClientNetworkManager.Instance.CommandsManager.AddCommand(answerTimeout);
            ClientNetworkManager.Instance.CommandsManager.AddCommand(loadQuestion);
            ClientNetworkManager.Instance.CommandsManager.AddCommand(basicExamGameEnd);
        }

        private void AttachEventsHooks()
        {
            ClientNetworkManager.Instance.OnConnectedEvent += this.OnConnected;
            ClientNetworkManager.Instance.OnDisconnectedEvent += this.OnDisconnectFromServer;

            this.questionUIController.OnAnswerClick += this.OnAnswerClick;

            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            ClientNetworkManager.Instance.OnConnectedEvent -= this.OnConnected;
            ClientNetworkManager.Instance.OnDisconnectedEvent -= this.OnDisconnectFromServer;
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
        }

        private void OnAnswerClick(object sender, AnswerEventArgs args)
        {
            var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
            answerSelectedCommand.AddOption("Answer", args.Answer);
            ClientNetworkManager.Instance.SendServerCommand(answerSelectedCommand);

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
            this.SecondsRemainingUIController.InvervalInSeconds = timeToAnswer;
            this.SecondsRemainingUIController.StartTimer();
            this.SecondsRemainingUIController.Paused = false;
        }

        public void ConnectTo(string ip)
        {
            this.ConnectingUI.SetActive(true);

            var status = ClientNetworkManager.Instance.ConnectToHost(ip);
            var statusMessage = NetworkErrorUtils.GetMessage(status);
            NotificationsController.Instance.AddNotification(Color.red, statusMessage);
        }

        public void Disconnect()
        {
            var status = ClientNetworkManager.Instance.Disconnect();
            var statusMessage = NetworkErrorUtils.GetMessage(status);
            NotificationsController.Instance.AddNotification(Color.red, statusMessage);
        }
    }

}
