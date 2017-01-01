namespace Assets.Scripts.Network
{
    using System;

    using UnityEngine;

    using IO;
    using NetworkManagers;

    using Commands;
    using Commands.GameData;
    using EventArgs;

    using EventArgs = System.EventArgs;

    public class GameDataSender : MonoBehaviour
    {
        public event EventHandler<ServerSentQuestionEventArgs> OnBeforeSend = delegate
            {
            };

        public event EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
            {
            };
    
        public GameDataIterator LocalGameData;
        public ServerNetworkManager NetworkManager;

        void Start()
        {
            this.NetworkManager.OnClientConnected += this.OnClientConnected;
            this.LocalGameData.OnMarkIncrease += this.OnMarkIncrease;
            this.LocalGameData.OnLoaded += this.OnGameDataLoaded;

            var getCurrentQuestionCommand = new GetCurrentQuestionCommand(this.LocalGameData, this.NetworkManager);
            var getNextQuestionCommand = new GetNextQuestionCommand(this.LocalGameData, this.NetworkManager);
                
            getCurrentQuestionCommand.OnBeforeSend += this.OnBeforeSentToClient;
            getNextQuestionCommand.OnBeforeSend += this.OnBeforeSentToClient;

            getCurrentQuestionCommand.OnSentQuestion += this.OnSentQuestionToClient;
            getNextQuestionCommand.OnSentQuestion += this.OnSentQuestionToClient;

            var commandsManager = this.NetworkManager.CommandsManager;

            commandsManager.AddCommand("GameDataGetQuestion", new GameDataGetQuestionRouterCommand(this.NetworkManager));
            commandsManager.AddCommand("GameDataGetCurrentQuestion", getCurrentQuestionCommand);
            commandsManager.AddCommand("GameDataGetNextQuestion", getNextQuestionCommand);
        }

        private void OnGameDataLoaded(object sender, EventArgs args)
        {
            var connectedClients = this.NetworkManager.ConnectedClientsConnectionId;

            for (int i = 0; i < connectedClients.Length; i++)
            {
                var clientId = connectedClients[i];
                this.SendLoadedGameData(clientId);
            }
        }

        private void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.LocalGameData.Loaded)
            {
                this.SendLoadedGameData(args.ConnectionId);
            }
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            var commandData = new NetworkCommandData("GameDataMark");
            commandData.AddOption("Mark", args.Mark.ToString());
            this.NetworkManager.SendAllClientsCommand(commandData);
        }

        private void OnSentQuestionToClient(object sender, ServerSentQuestionEventArgs args)
        {
            this.OnSentQuestion(this, args);
        }

        private void OnBeforeSentToClient(object sender, ServerSentQuestionEventArgs args)
        {
            this.OnBeforeSend(this, args);
        }

        private void SendLoadedGameData(int connectionId)
        {
            var loadedGameDataCommand = new NetworkCommandData("LoadedGameData");
            loadedGameDataCommand.AddOption("LevelCategory", this.LocalGameData.LevelCategory);
            this.NetworkManager.SendClientCommand(connectionId, loadedGameDataCommand);
        }
    }
}