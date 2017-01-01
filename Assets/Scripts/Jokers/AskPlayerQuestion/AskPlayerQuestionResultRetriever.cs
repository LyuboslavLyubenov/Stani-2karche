namespace Assets.Scripts.Jokers.AskPlayerQuestion
{
    using System;
    using System.Timers;

    using Commands;
    using Commands.Client;
    using Commands.Jokers;
    using EventArgs;
    using Network.NetworkManagers;
    using Utils;
    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class AskPlayerQuestionResultRetriever : ExtendedMonoBehaviour
    {
        private const int SettingsReceiveTimeoutInSeconds = 5;

        public event EventHandler<AskPlayerResponseEventArgs> OnReceivedAnswer = delegate
            {
            };

        public event EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
            {
            };

        public event EventHandler OnReceiveAnswerTimeout = delegate
            {
            };

        public event EventHandler OnReceiveSettingsTimeout = delegate
            {
            };

        private ClientNetworkManager networkManager;

        private Timer timer;

        public AskPlayerQuestionResultRetriever(ClientNetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }
        
        private void _OnReceivedSettings(int timeToAnswerInSeconds)
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

        private void Timer_OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    this.timer.Close();
                    this.networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
                    this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
                });
        }

        private void _OnReceivedAnswer(string username, string answer)
        {
            this.OnReceivedAnswer(this, new AskPlayerResponseEventArgs(username, answer));
        }

        private void Timer_OnReceiveAnswerTimeout(object sender, ElapsedEventArgs args)
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
