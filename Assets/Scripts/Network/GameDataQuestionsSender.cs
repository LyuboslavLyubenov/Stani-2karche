namespace Network
{

    using System;

    using Commands;
    using Commands.GameData;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class GameDataQuestionsSender : IGameDataQuestionsSender
    {
        public event EventHandler<ServerSentQuestionEventArgs> OnBeforeSend = delegate
            {
            };

        public event EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
            {
            };
    
        private IGameDataIterator gameDataIterator;
        private IServerNetworkManager networkManager;

        public GameDataQuestionsSender(IGameDataIterator gameDataIterator, IServerNetworkManager networkManager)
        {
            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.gameDataIterator = gameDataIterator;
            this.networkManager = networkManager;
            
            this.networkManager.OnClientConnected += this.OnClientConnected;
            this.gameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.gameDataIterator.OnLoaded += this.OnGameDataLoaded;

            this.InitializeCommands();
        }

        private void InitializeCommands()
        {
            var getCurrentQuestionCommand = new GetCurrentQuestionCommand(this.gameDataIterator, this.networkManager);
            var getNextQuestionCommand = new GetNextQuestionCommand(this.gameDataIterator, this.networkManager);

            getCurrentQuestionCommand.OnBeforeSend += this.OnBeforeSentToClient;
            getNextQuestionCommand.OnBeforeSend += this.OnBeforeSentToClient;

            getCurrentQuestionCommand.OnSentQuestion += this.OnSentQuestionToClient;
            getNextQuestionCommand.OnSentQuestion += this.OnSentQuestionToClient;

            var commandsManager = this.networkManager.CommandsManager;

            commandsManager.AddCommand("GameDataGetQuestion", new GameDataGetQuestionRouterCommand(this.networkManager));
            commandsManager.AddCommand("GameDataGetCurrentQuestion", getCurrentQuestionCommand);
            commandsManager.AddCommand("GameDataGetNextQuestion", getNextQuestionCommand);
        }

        private void OnGameDataLoaded(object sender, EventArgs args)
        {
            var connectedClients = this.networkManager.ConnectedClientsConnectionId;

            for (int i = 0; i < connectedClients.Length; i++)
            {
                var clientId = connectedClients[i];
                this.SendLoadedGameData(clientId);
            }
        }

        private void OnClientConnected(object sender, ClientConnectionIdEventArgs args)
        {
            if (this.gameDataIterator.Loaded)
            {
                this.SendLoadedGameData(args.ConnectionId);
            }
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            var commandData = NetworkCommandData.From<GameDataMarkCommand>();
            commandData.AddOption("Mark", args.Mark.ToString());
            this.networkManager.SendAllClientsCommand(commandData);
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
            var loadedGameDataCommand = NetworkCommandData.From<LoadedGameDataCommand>();
            loadedGameDataCommand.AddOption("LevelCategory", this.gameDataIterator.LevelCategory);
            this.networkManager.SendClientCommand(connectionId, loadedGameDataCommand);
        }
    }
}