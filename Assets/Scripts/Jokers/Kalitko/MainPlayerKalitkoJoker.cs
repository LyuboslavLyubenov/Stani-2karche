namespace Jokers.Kalitko
{
    using System;

    using Assets.Scripts.Commands.Jokers.JokerElection;

    using Commands;
    using Commands.Jokers.JokerElection.KalitkoJoker;
    using Commands.Jokers.Selected;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    public class MainPlayerKalitkoJoker : IJoker
    {
        public event EventHandler OnActivated = delegate {};
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate {};
        public event EventHandler OnFinishedExecution = delegate {};

        private readonly IClientNetworkManager networkManager;

        private readonly PlayersSelectJokerTimeoutCommand selectedTimeoutCommand = new PlayersSelectJokerTimeoutCommand();
        private readonly AllPlayersSelectedJokerCommand allPlayersSelectedCommand = new AllPlayersSelectedJokerCommand();

        public Sprite Image { get; private set; }

        public bool Activated { get; private set; }

        public MainPlayerKalitkoJoker(IClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;

            throw new NotImplementedException();

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/Kalitko");
        }
        
        private void AddCommands()
        {
            this.allPlayersSelectedCommand.OnExecuted += OnAllPlayersSelected;
            this.selectedTimeoutCommand.OnExecuted += OnPlayersSelectedTimeout;

            this.networkManager.CommandsManager.AddCommand("SelectKalitkoJokerTimeout", this.selectedTimeoutCommand);
            this.networkManager.CommandsManager.AddCommand("AllPlayersSelectedKalitkoJoker", this.allPlayersSelectedCommand);
        }

        private void RemoveCommands()
        {
            if (this.networkManager.CommandsManager.Exists("SelectKalitkoJokerTimeout"))
            {
                this.networkManager.CommandsManager.RemoveCommand("SelectKalitkoJokerTimeout");
            }

            if (this.networkManager.CommandsManager.Exists("AllPlayersSelectedKalitkoJoker"))
            {
                this.networkManager.CommandsManager.RemoveCommand("AllPlayersSelectedKalitkoJoker");
            }
        }

        private void OnAllPlayersSelected(object sender, EventArgs eventArgs)
        {
            this.RemoveCommands();

            this.OnFinishedExecution(this, EventArgs.Empty);
            this.Activated = false;
        }

        private void OnPlayersSelectedTimeout(object sender, EventArgs args)
        {
            this.RemoveCommands();

            var exception = new TimeoutException("Not all players selected kalitko joker");
            this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
        }

        public void Activate()
        {
            var selectedJokerCommand = NetworkCommandData.From<SelectedKalitkoJokerCommand>();
            this.networkManager.SendServerCommand(selectedJokerCommand);

            this.AddCommands();

            this.OnActivated(this, EventArgs.Empty);
            this.Activated = true;
        }
    }
}