// ReSharper disable ArrangeTypeMemberModifiers
namespace Assets.Scripts.Network.Servers
{
    using System;

    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.Leaderboard;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Interfaces.Statistics;
    using Assets.Scripts.Jokers.Routers;

    using Leaderboard;
    using TcpSockets;

    using Commands;
    using Commands.Client;
    using Commands.Jokers;
    using Commands.Server;
    using Controllers;
    using DTOs;
    using EventArgs;
    using Interfaces;
    using IO;
    using Jokers;

    using NetworkManagers;
    using Statistics;
    using Utils;
    using Utils.Unity;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using EventArgs = System.EventArgs;

    public class BasicExamServer : ExtendedMonoBehaviour, IGameServer, IDisposable
    {
        private const float DefaultChanceToAddRandomJokerOnMarkChange = 0.08f;

        private const int DefaultServerMaxPlayers = 40;

        public event EventHandler OnGameOver = delegate
            {
            };

        public event EventHandler<AnswerEventArgs> OnMainPlayerSelectedAnswer = delegate
            {
            };
        
        public ConnectedClientsUIController ConnectedClientsUIController;
        public BasicExamClientOptionsUIController ClientOptionsUIController;
        
        public int RemainingTimetoAnswerInSeconds
        {
            get
            {
                return this.remainingTimeToAnswerMainQuestion;
            }
        }

        public bool IsGameOver
        {
            get;
            private set;
        }

        public MainPlayerData MainPlayerData
        {
            get;
            private set;
        }

        public IGameDataIterator GameDataIterator
        {
            get; private set;
        }

        public IGameDataQuestionsSender GameDataQuestionsSender
        {
            get; private set;
        }
        
        private MainPlayerJokersDataSynchronizer mainPlayerJokersDataSynchronizer;

        private float chanceToAddRandomJoker = DefaultChanceToAddRandomJokerOnMarkChange;

        private int remainingTimeToAnswerMainQuestion = -1;
        // when using joker
        private bool paused = false;

        private bool playerSentAnswer = false;

        private ISimpleQuestion lastQuestion;

        private IBasicExamStatisticsCollector statisticsCollector = null;

        private ILeaderboardDataManipulator leaderboardDataManipulator = null;

        private IAskPlayerQuestionRouter askPlayerQuestionRouter = null;
        private IAudienceAnswerPollRouter audiencePollRouter = null;
        private IDisableRandomAnswersRouter disableRandomAnswersJokerRouter = null;
        private IAddRandomJokerRouter addRandomJokerRouter = null;
        private ILeaderboardSender leaderboardSender = null;
        private IGameDataExtractor gameDataExtractor = null;

        private ICreatedGameInfoSender gameInfoSender = null;
        private ISimpleTcpServer tcpServer;
        private ISimpleTcpClient tcpClient;

        void Awake()
        {
            var threadUtils = ThreadUtils.Instance;//Initialize
            var serverNetworkManager = ServerNetworkManager.Instance;
            
            this.MainPlayerData = new MainPlayerData(serverNetworkManager);
            this.mainPlayerJokersDataSynchronizer = new MainPlayerJokersDataSynchronizer(serverNetworkManager, this.MainPlayerData);

            this.tcpServer = new SimpleTcpServer(7772);
            this.tcpClient = new SimpleTcpClient();
            this.gameInfoSender = new CreatedGameInfoSender(this.tcpClient, this.tcpServer, GameInfoFactory.Instance, ServerNetworkManager.Instance, this);
            
            this.gameDataExtractor = new GameDataExtractor();
            this.GameDataIterator = new GameDataIterator(this.gameDataExtractor);
            this.GameDataQuestionsSender = new GameDataQuestionsSender(this.GameDataIterator, serverNetworkManager);

            this.leaderboardDataManipulator = new LeaderboardDataManipulator();
            this.disableRandomAnswersJokerRouter = new DisableRandomAnswersJokerRouter(serverNetworkManager, MainPlayerData);
            this.addRandomJokerRouter = new AddRandomJokerRouter(serverNetworkManager, MainPlayerData.JokersData);
           
            this.askPlayerQuestionRouter = new AskPlayerQuestionRouter(serverNetworkManager, this.GameDataIterator);
            this.audiencePollRouter = new AudienceAnswerPollRouter(serverNetworkManager, this.GameDataIterator);

            this.statisticsCollector = new BasicExamStatisticsCollector(serverNetworkManager, this, this.GameDataQuestionsSender, this.GameDataIterator);

            this.leaderboardSender = new LeaderboardSender(serverNetworkManager, this.leaderboardDataManipulator);
            
            this.LoadServerSettings();
            this.InitializeCommands();
            this.AttachEventHandlers();

            this.MainPlayerData.JokersData.AddJoker<HelpFromFriendJoker>();
            this.MainPlayerData.JokersData.AddJoker<AskAudienceJoker>();
            this.MainPlayerData.JokersData.AddJoker<DisableRandomAnswersJoker>();
        }
        
        void Start()
        {
            this.CoroutineUtils.RepeatEverySeconds(1f, () =>
                {
                    if (!ServerNetworkManager.Instance.IsRunning || this.IsGameOver || this.paused || this.remainingTimeToAnswerMainQuestion == -1)
                    {
                        return;
                    }

                    this.UpdateRemainingTime();
                });

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        void OnApplicationQuit()
        {
            this.Dispose();
        }

        private void OnConnectedClientSelected(int connectionId)
        {
            this.ClientOptionsUIController.gameObject.SetActive(true);
            this.CoroutineUtils.WaitForFrames(1, () =>
            {
                var username = ServerNetworkManager.Instance.GetClientUsername(connectionId);
                var clientData = new ConnectedClientData(connectionId, username);
                var role =
                    (this.MainPlayerData.IsConnected && this.MainPlayerData.ConnectionId == connectionId)
                        ?
                        BasicExamClientRole.MainPlayer
                        :
                        BasicExamClientRole.Audience;

                this.ClientOptionsUIController.Set(clientData, role);
            });
        }

        private void OnMainPlayerSurrender()
        {
            PlayerPrefs.SetString("Surrender", "true");
            this.EndGame();
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.askPlayerQuestionRouter.Deactivate();
            this.audiencePollRouter.Deactivate();
        }

        private void OnReceivedSelectedAnswer(int clientId, string answer)
        {
            if (this.IsGameOver ||
                !this.MainPlayerData.IsConnected ||
                this.MainPlayerData.ConnectionId != clientId ||
                this.paused)
            {
                return;
            }
            
            var isCorrect = (answer == this.lastQuestion.CorrectAnswer);

            if (isCorrect)
            {
                this.OnMainPlayerAnsweredCorrectly();
            }
            else
            {
                this.OnMainPlayerAnsweredIncorrectly();
            }
            
            this.playerSentAnswer = true;
        }

        private void OnMainPlayerAnsweredCorrectly()
        {
            if (UnityEngine.Random.value < this.chanceToAddRandomJoker)
            {
                var jokers = JokerUtils.AllJokersTypes;
                this.addRandomJokerRouter.Activate(this.MainPlayerData.ConnectionId, jokers);
            }
        }

        private void OnMainPlayerAnsweredIncorrectly()
        {
            this.CoroutineUtils.WaitForFrames(1, this.EndGame);
        }

        private void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                if (args.ClientId != this.MainPlayerData.ConnectionId)
                {
                    throw new Exception("Client id " + args.ClientId + " doesnt have premission to get next question.");
                }

                if (!this.playerSentAnswer)
                {
                    throw new Exception("MainPlayer must answer current question before requesting new one.");
                }
            }
        }

        private void OnMainPlayerSelectedJoker(object sender, EventArgs args)
        {
            var jokerName = sender.GetType().Name
                .Replace("Selected", "")
                .Replace("Command", "")
                .ToUpperInvariant();

            if (jokerName == typeof(DisableRandomAnswersJoker).Name.ToUpperInvariant())
            {
                return;
            }

            this.paused = true;
        }

        private void OnLoadedGameData(object sender, EventArgs args)
        {
            this.remainingTimeToAnswerMainQuestion = this.GameDataIterator.SecondsForAnswerQuestion;
        }

        private void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                this.remainingTimeToAnswerMainQuestion = this.GameDataIterator.SecondsForAnswerQuestion;
                this.playerSentAnswer = false;
            }

            this.lastQuestion = args.Question;
        }

        private void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.IsGameOver)
            {
                this.SendEndGameInfo();
            }
        }

        private void OnFinishedJokerExecution()
        {
            this.paused = false;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            this.Dispose();
        }

        private void LoadServerSettings()
        {
            int serverMaxPlayers = DefaultServerMaxPlayers;

            if (PlayerPrefsEncryptionUtils.HasKey("ServerMaxPlayers"))
            {
                serverMaxPlayers = int.Parse(PlayerPrefsEncryptionUtils.GetString("ServerMaxPlayers"));    
            }

            ServerNetworkManager.Instance.MaxConnections = serverMaxPlayers;    
        }

        private void AttachEventHandlers()
        {
            this.MainPlayerData.OnDisconnected += this.OnMainPlayerDisconnected;

            this.GameDataIterator.OnLoaded += this.OnLoadedGameData;
            this.GameDataQuestionsSender.OnSentQuestion += this.OnSentQuestion;
            this.GameDataQuestionsSender.OnBeforeSend += this.OnBeforeSendQuestion;

            ServerNetworkManager.Instance.OnClientConnected += this.OnClientConnected;

            this.askPlayerQuestionRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();
            this.audiencePollRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();

            this.ConnectedClientsUIController.OnSelectedPlayer += (sender, args) => this.OnConnectedClientSelected(args.ConnectionId);
        }
        
        private void InitializeCommands()
        {
            AvailableCategoriesCommandsInitializator.Initialize(ServerNetworkManager.Instance, this.gameDataExtractor, this.leaderboardDataManipulator);

            var selectedAnswerCommand = new SelectedAnswerCommand(this.OnReceivedSelectedAnswer);
            var selectedAskPlayerQuestionCommand = new SelectedAskPlayerQuestionCommand(ServerNetworkManager.Instance, this.MainPlayerData, this.askPlayerQuestionRouter, 60);
            var selectedAudiencePollCommand = new SelectedAudiencePollCommand(this.MainPlayerData, this.audiencePollRouter, 60);
            var selectedFifthyFifthyChanceCommand = new SelectedDisableRandomAnswersJokerCommand(this.MainPlayerData, this.disableRandomAnswersJokerRouter, 2);
            var surrenderCommand = new SurrenderBasicExamOneTimeCommand(this.MainPlayerData, this.OnMainPlayerSurrender);
            var selectedJokerCommands = new INetworkOperationExecutedCallback[] { selectedAudiencePollCommand, selectedFifthyFifthyChanceCommand, selectedAskPlayerQuestionCommand };

            for (var i = 0; i < selectedJokerCommands.Length; i++)
            {
                selectedJokerCommands[i].OnExecuted += this.OnMainPlayerSelectedJoker;
            }

            ServerNetworkManager.Instance.CommandsManager.AddCommand("AnswerSelected", selectedAnswerCommand);
            ServerNetworkManager.Instance.CommandsManager.AddCommand(selectedAskPlayerQuestionCommand);
            ServerNetworkManager.Instance.CommandsManager.AddCommand(selectedAudiencePollCommand);
            ServerNetworkManager.Instance.CommandsManager.AddCommand(selectedFifthyFifthyChanceCommand);
            ServerNetworkManager.Instance.CommandsManager.AddCommand("Surrender", surrenderCommand);
        }


        private void UpdateRemainingTime()
        {
            if (!this.MainPlayerData.IsConnected || this.paused)
            {
                return;
            }

            this.remainingTimeToAnswerMainQuestion -= 1;

            if (this.remainingTimeToAnswerMainQuestion <= 0)
            {
                this.EndGame();
            }
        }

        private void SendEndGameInfo()
        {
            var commandData = NetworkCommandData.From<BasicExamGameEndCommand>();
            commandData.AddOption("Mark", this.GameDataIterator.CurrentMark.ToString());
            ServerNetworkManager.Instance.SendAllClientsCommand(commandData);
        }

        private void SavePlayerScoreToLeaderboard()
        {
            var mainPlayerName = this.MainPlayerData.Username;
            var playerScore = new PlayerScore(mainPlayerName, this.statisticsCollector.PlayerScore, DateTime.Now);

            this.leaderboardDataManipulator.SavePlayerScore(playerScore);
        }

        private void ExportStatistics()
        {
            new BasicExamGeneralStatiticsExporter(this.statisticsCollector).Export();
            new BasicExamGameDataStatisticsExporter(this.statisticsCollector, this.GameDataIterator).Export();
        }

        public void EndGame()
        {
            this.SavePlayerScoreToLeaderboard();
            this.SendEndGameInfo();
            this.ExportStatistics();

            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            this.OnMainPlayerSelectedAnswer = null;
            this.OnGameOver = null;

            this.askPlayerQuestionRouter.Dispose();
            this.audiencePollRouter.Dispose();
            this.gameInfoSender.Dispose();

            this.MainPlayerData = null;
            this.GameDataQuestionsSender = null;
            this.GameDataIterator = null;

            this.lastQuestion = null;
            this.statisticsCollector = null;
            this.askPlayerQuestionRouter = null;
            this.audiencePollRouter = null;
            this.disableRandomAnswersJokerRouter = null;
            this.addRandomJokerRouter = null;
            this.leaderboardDataManipulator = null;
            this.mainPlayerJokersDataSynchronizer = null;

            ServerNetworkManager.Instance.CommandsManager.RemoveAllCommands();
            ServerNetworkManager.Instance.Dispose();
        }
    }
}