namespace Assets.Scripts.Network
{
    using System;

    using Utils;

    using IO;
    using NetworkManagers;

    using Commands;
    using Commands.GameData;
    using EventArgs;

    using EventArgs = System.EventArgs;

    public class GameDataSender
    {
        public event EventHandler<ServerSentQuestionEventArgs> OnBeforeSend = delegate
            {
            };

        public event EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
            {
            };
    
        private GameDataIterator gameDataIterator;
        private ServerNetworkManager networkManager;

        public GameDataSender(GameDataIterator gameDataIterator, ServerNetworkManager networkManager)
        {
            ValidationUtils.ValidateObjectNotNull(gameDataIterator, "gameDataIterator");
            ValidationUtils.ValidateObjectNotNull(networkManager, "networkManager");

            this.gameDataIterator = gameDataIterator;
            this.networkManager = networkManager;
            
            this.networkManager.OnClientConnected += this.OnClientConnected;
            this.gameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.gameDataIterator.OnLoaded += this.OnGameDataLoaded;

            this.IntializeCommands();
        }

        void IntializeCommands()
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

        private void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.gameDataIterator.Loaded)
            {
                this.SendLoadedGameData(args.ConnectionId);
            }
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            var commandData = new NetworkCommandData("GameDataMark");
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
            var loadedGameDataCommand = new NetworkCommandData("LoadedGameData");
            loadedGameDataCommand.AddOption("LevelCategory", this.gameDataIterator.LevelCategory);
            this.networkManager.SendClientCommand(connectionId, loadedGameDataCommand);
        }
    }
}