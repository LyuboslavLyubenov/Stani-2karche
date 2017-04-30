using IAskClientQuestionResultRetriever = Interfaces.Network.Jokers.IAskClientQuestionResultRetriever;

namespace Jokers
{

    using System;
    using System.Linq;

    using Assets.Scripts.Interfaces;

    using Commands;
    using Commands.Client;
    using Commands.Server;

    using Controllers;

    using DTOs;

    using EventArgs;

    using Exceptions;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using Localization;

    using Notifications;

    using UnityEngine;

    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class HelpFromFriendJoker : IJoker
    {
        public event EventHandler<AnswerEventArgs> OnFriendAnswered = delegate
            {
            };

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public event EventHandler OnFinishedExecution = delegate
            {
            };

        private readonly CallAFriendUIController callAFriendUIController;
        private readonly IClientNetworkManager networkManager;

        private readonly IAskClientQuestionResultRetriever resultRetriever;

        private readonly GameObject callAFriendUI;
        private readonly GameObject friendAnswerUI;
        private readonly GameObject waitingToAnswerUI;
        private readonly GameObject loadingUI;

        public Sprite Image
        {
            get;
            private set;
        }

        public bool Activated
        {
            get;
            private set;
        }

        public HelpFromFriendJoker(IClientNetworkManager networkManager,
                                   IAskClientQuestionResultRetriever resultRetriever,
                                   GameObject callAFriendUI,
                                   GameObject friendAnswerUI,
                                   GameObject waitingToAnswerUI,
                                   GameObject loadingUI)
        {

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (callAFriendUI == null)
            {
                throw new ArgumentNullException("callAFriendUI");
            }

            if (friendAnswerUI == null)
            {
                throw new ArgumentNullException("friendAnswerUI");
            }

            if (waitingToAnswerUI == null)
            {
                throw new ArgumentNullException("waitingToAnswerUI");
            }

            if (loadingUI == null)
            {
                throw new ArgumentNullException("loadingUI");
            }

            this.networkManager = networkManager;
            this.resultRetriever = resultRetriever;
            this.callAFriendUI = callAFriendUI;
            this.friendAnswerUI = friendAnswerUI;
            this.waitingToAnswerUI = waitingToAnswerUI;
            this.loadingUI = loadingUI;

            this.callAFriendUIController = callAFriendUI.GetComponent<CallAFriendUIController>();

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/HelpFromFriend");
        }

        private void OnReceivedConnectedClientsIdsNames(OnlineClientsData_DTO connectedClientsData)
        {
            var connectedClientsIdsNames = connectedClientsData.OnlinePlayers.ToDictionary(c => c.ConnectionId, c => c.Username);
            connectedClientsIdsNames.Add(NetworkCommandData.CODE_Option_ClientConnectionId_AI, LanguagesManager.Instance.GetValue("Computer"));

            this.loadingUI.SetActive(false);
            this.callAFriendUI.SetActive(true);

            this.callAFriendUIController.SetContacts(connectedClientsIdsNames);
            this.callAFriendUIController.OnCalledPlayer += this.OnCalledPlayer;
        }

        private void OnCalledPlayer(object sender, PlayerCalledEventArgs args)
        {
            this.callAFriendUI.SetActive(false);
            this.loadingUI.SetActive(true);

            this.resultRetriever.OnReceivedSettings += this.OnReceivedSettings;
            this.resultRetriever.OnReceiveSettingsTimeout += this.OnReceiveSettingsTimeout;
            this.resultRetriever.OnReceivedAnswer += this.OnReceivedAnswer;
            this.resultRetriever.OnReceiveAnswerTimeout += this.OnReceiveAnswerTimeout;

            this.resultRetriever.Activate(args.PlayerConnectionId);
        }

        private void OnReceivedSettings(object sender, JokerSettingsEventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(true);
            this.waitingToAnswerUI.GetComponent<DisableAfterDelay>().InvervalInSeconds = args.TimeToAnswerInSeconds;
        }

        private void OnReceiveSettingsTimeout(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Errors/NetworkErrors/Timeout");
            NotificationsController.Instance.AddNotification(Color.red, message);

            this.Activated = false;

            if (this.OnError != null)
            {
                this.OnError(this, new UnhandledExceptionEventArgs(new JokerSettingsTimeoutException(), true));
            }
        }

        private void OnReceivedAnswer(object sender, AskClientQuestionResponseEventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);
            this.friendAnswerUI.SetActive(true);
            this.friendAnswerUI.GetComponent<FriendAnswerUIController>()
                .SetResponse(args.Username, args.Answer);
            
            this.Activated = false;
            
            this.OnFriendAnswered(this, new AnswerEventArgs(args.Answer, null));

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }

        private void OnReceiveAnswerTimeout(object sender, EventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Errors/NetworkErrors/Timeout");
            NotificationsController.Instance.AddNotification(Color.red, message);

            this.Activated = false;

            if (this.OnError != null)
            {
                this.OnError(this, new UnhandledExceptionEventArgs(new TimeoutException(), true));
            }
        }

        private void BeginReceiveConnectedClientsIdsNames()
        {
            var commandData = NetworkCommandData.From<ServerSendConnectedClientsIdsNamesCommand>();
            this.networkManager.SendServerCommand(commandData);
            this.networkManager.CommandsManager.AddCommand("ConnectedClientsIdsNames", new ConnectedClientsDataCommand(this.OnReceivedConnectedClientsIdsNames));
        }

        public void Activate()
        {
            this.loadingUI.SetActive(true);
            this.BeginReceiveConnectedClientsIdsNames();
            this.Activated = true;

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }
        }
    }
}