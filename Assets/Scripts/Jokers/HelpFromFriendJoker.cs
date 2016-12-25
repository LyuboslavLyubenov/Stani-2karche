using System;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Jokers
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Controllers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers.AskPlayerQuestion;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class HelpFromFriendJoker : IJoker
    {
        //0% = 0f, 100% = 1f
        const float ChanceForGeneratingCorrectAnswer = 0.85f;

        public EventHandler<AnswerEventArgs> OnFriendAnswered = delegate
            {
            };

        CallAFriendUIController callAFriendUIController;

        ClientNetworkManager networkManager;

        GameObject callAFriendUI;
        GameObject friendAnswerUI;
        GameObject waitingToAnswerUI;
        GameObject loadingUI;

        public Sprite Image
        {
            get;
            private set;
        }

        public EventHandler OnActivated
        {
            get;
            set;
        }

        public bool Activated
        {
            get;
            private set;
        }

        public HelpFromFriendJoker(ClientNetworkManager networkManager, 
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
            this.callAFriendUI = callAFriendUI;
            this.friendAnswerUI = friendAnswerUI;
            this.waitingToAnswerUI = waitingToAnswerUI;
            this.loadingUI = loadingUI;

            this.callAFriendUIController = callAFriendUI.GetComponent<CallAFriendUIController>();

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/HelpFromFriend");
        }

        void OnReceivedConnectedClientsIdsNames(OnlineClientsData_Serializable connectedClientsData)
        {
            var connectedClientsIdsNames = connectedClientsData.OnlinePlayers.ToDictionary(c => c.ConnectionId, c => c.Username);
            connectedClientsIdsNames.Add(NetworkCommandData.CODE_Option_ClientConnectionId_AI, LanguagesManager.Instance.GetValue("Computer"));

            this.loadingUI.SetActive(false);
            this.callAFriendUI.SetActive(true);
        
            this.callAFriendUIController.SetContacts(connectedClientsIdsNames);
            this.callAFriendUIController.OnCalledPlayer += this.OnCalledPlayer;
        }

        void OnCalledPlayer(object sender, PlayerCalledEventArgs args)
        {
            this.callAFriendUI.SetActive(false);
            this.loadingUI.SetActive(true);

            var resultRetriever = AskPlayerQuestionResultRetriever.Instance;

            resultRetriever.OnReceivedSettings += this.OnReceivedSettings;
            resultRetriever.OnReceiveSettingsTimeout += this.OnReceiveSettingsTimeout;
            resultRetriever.OnReceivedAnswer += this.OnReceivedAnswer;
            resultRetriever.OnReceiveAnswerTimeout += this.OnReceiveAnswerTimeout;

            resultRetriever.Activate(args.PlayerConnectionId);
        }

        void OnReceivedSettings(object sender, JokerSettingsEventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(true);
            this.waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = args.TimeToAnswerInSeconds;
        }

        void OnReceiveSettingsTimeout(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Errors/NetworkErrors/Timeout");
            NotificationsServiceController.Instance.AddNotification(Color.red, message);

            this.Activated = false;
        }

        void OnReceivedAnswer(object sender, AskPlayerResponseEventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);
            this.friendAnswerUI.SetActive(true);
            this.friendAnswerUI.GetComponent<FriendAnswerUIController>().SetResponse(args.Username, args.Answer);

            this.Activated = false;
        }

        void OnReceiveAnswerTimeout(object sender, EventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Errors/NetworkErrors/Timeout");
            NotificationsServiceController.Instance.AddNotification(Color.red, message);

            this.Activated = false;
        }

        void BeginReceiveConnectedClientsIdsNames()
        {
            var commandData = new NetworkCommandData("ConnectedClientsIdsNames");
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