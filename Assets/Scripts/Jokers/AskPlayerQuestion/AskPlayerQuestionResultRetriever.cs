using System;
using System.Timers;

using UnityEngine;

namespace Assets.Scripts.Jokers.AskPlayerQuestion
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    using EventArgs = System.EventArgs;

    public class AskPlayerQuestionResultRetriever : ExtendedMonoBehaviour
    {
        const int SettingsReceiveTimeoutInSeconds = 5;

        static AskPlayerQuestionResultRetriever instance;

        public static AskPlayerQuestionResultRetriever Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject();
                    obj.name = typeof(AskPlayerQuestionResultRetriever).Name;
                    instance = obj.AddComponent<AskPlayerQuestionResultRetriever>();
                }

                return instance;
            }
        }

        public EventHandler<AskPlayerResponseEventArgs> OnReceivedAnswer = delegate
            {
            };

        public EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
            {
            };

        public EventHandler OnReceiveAnswerTimeout = delegate
            {
            };

        public EventHandler OnReceiveSettingsTimeout = delegate
            {
            };
    
        ClientNetworkManager networkManager;

        Timer timer;

        void Awake()
        {
            this.networkManager = GameObject.FindObjectOfType<ClientNetworkManager>();     
        }

        void _OnReceivedSettings(int timeToAnswerInSeconds)
        {
            this.timer.Stop();
            this.timer.Close();

            var responseCommand = new AskPlayerResponseCommand(this._OnReceivedAnswer);
            this.networkManager.CommandsManager.AddCommand(responseCommand);

            this.timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.Timer_OnReceiveAnswerTimeout;

            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        void Timer_OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    this.timer.Close();
                    this.networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
                    this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
                });
        }

        void _OnReceivedAnswer(string username, string answer)
        {
            this.OnReceivedAnswer(this, new AskPlayerResponseEventArgs(username, answer));
        }

        void Timer_OnReceiveAnswerTimeout(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    this.timer.Close();
                    this.networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
                    this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
                });
        }

        public void Activate(int playerConnectionId)
        {
            var selected = NetworkCommandData.From<SelectedAskPlayerQuestionCommand>();
            selected.AddOption("PlayerConnectionId", playerConnectionId.ToString());

            this.networkManager.SendServerCommand(selected);

            var receivedSettingsCommand = new HelpFromFriendJokerSettingsCommand(this._OnReceivedSettings);
            this.networkManager.CommandsManager.AddCommand(receivedSettingsCommand);

            this.timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.Timer_OnReceiveSettingsTimeout;
        }
    }

}
