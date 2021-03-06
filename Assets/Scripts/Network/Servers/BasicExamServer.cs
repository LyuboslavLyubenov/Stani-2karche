﻿// ReSharper disable ArrangeTypeMemberModifiers

namespace Network.Servers
{

    using System;

    using Assets.Scripts.Network.GameInfo.New;

    using Commands;
    using Commands.Client;
    using Commands.Jokers.Selected;
    using Commands.Server;
    using Commands.GameData;

    using Controllers;

    using DTOs;

    using EventArgs;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.Leaderboard;
    using Interfaces.Network.NetworkManager;
    using Interfaces.Statistics;

    using IO;

    using Jokers;
    using Jokers.Routers;

    using Network.Leaderboard;
    using Network.NetworkManagers;

    using Statistics;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Utils;
    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class BasicExamServer : ExtendedMonoBehaviour, IGameServer, IDisposable
    {
        private const float DefaultChanceToAddRandomJokerOnMarkChange = 0.08f;
        private const int DefaultServerMaxPlayers = 30;

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

        private IAskPlayerQuestionJokerRouter askPlayerQuestionRouter = null;
        private IHelpFromAudienceJokerRouter audiencePollRouter = null;
        private IDisableRandomAnswersRouter disableRandomAnswersJokerRouter = null;
        private IAddRandomJokerRouter addRandomJokerRouter = null;
        private ILeaderboardSender leaderboardSender = null;
        private IGameDataExtractor gameDataExtractor = null;

        private ICreatedGameInfoSender gameInfoSender = null;

        private void AllocateMoreMemory()
        {
            var tmp = new System.Object[1024 * 512];

            // make allocations in smaller blocks to avoid them to be treated in a special way, which is designed for large blocks
            for (int i = 0; i < tmp.Length; i++)
                tmp[i] = new byte[1024];

            // release reference
            tmp = null;
        }

        void Awake()
        {
            this.AllocateMoreMemory();
            this.CoroutineUtils.WaitForSeconds(60, System.GC.Collect);

            var threadUtils = ThreadUtils.Instance;//Initialize
        }

        void Start()
        {
            var serverNetworkManager = ServerNetworkManager.Instance;

            //if (!serverNetworkManager.IsRunning)
            //{
            //    serverNetworkManager.StartServer();
            //}

            this.MainPlayerData = new MainPlayerData(serverNetworkManager);
            this.mainPlayerJokersDataSynchronizer = new MainPlayerJokersDataSynchronizer(serverNetworkManager, this.MainPlayerData);

            this.gameInfoSender = new CreatedGameInfoSender(ServerNetworkManager.Instance, this);

            this.gameDataExtractor = 
                new GameDataExtractor() 
                {
                    ShuffleAnswers = true,
                    ShuffleQuestions = true
                };
            
            this.GameDataIterator = new GameDataIterator(this.gameDataExtractor);
            this.GameDataQuestionsSender = new GameDataQuestionsSender(this.GameDataIterator, serverNetworkManager);

            this.leaderboardDataManipulator = new LeaderboardDataManipulator();
            this.disableRandomAnswersJokerRouter = new DisableRandomAnswersJokerRouter(serverNetworkManager, this.GameDataIterator);
            this.addRandomJokerRouter = new AddRandomJokerRouter(serverNetworkManager, this.MainPlayerData.JokersData);

            this.askPlayerQuestionRouter = new AskPlayerQuestionJokerRouter(serverNetworkManager, this.GameDataIterator, new AskClientQuestionRouter(serverNetworkManager));

            var answerPollRouter = new AnswerPollRouter(serverNetworkManager);
            this.audiencePollRouter = new HelpFromAudienceJokerRouter(serverNetworkManager, this.GameDataIterator, answerPollRouter);

            this.statisticsCollector = new BasicExamStatisticsCollector(serverNetworkManager, this, this.GameDataQuestionsSender, this.GameDataIterator);

            this.leaderboardSender = new LeaderboardSender(serverNetworkManager, this.leaderboardDataManipulator);

            this.LoadServerSettings();
            this.InitializeCommands();
            this.AttachEventHandlers();

            this.MainPlayerData.JokersData.AddJoker<HelpFromFriendJoker>();
            this.MainPlayerData.JokersData.AddJoker<AskAudienceJoker>();
            this.MainPlayerData.JokersData.AddJoker<DisableRandomAnswersJoker>();

            this.CoroutineUtils.RepeatEverySeconds(1f, () =>
                {
                    if (!ServerNetworkManager.Instance.IsRunning || this.IsGameOver || this.paused || this.remainingTimeToAnswerMainQuestion == -1)
                    {
                        return;
                    }

                    this.UpdateRemainingTime();
                });

            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        void OnApplicationQuit()
        {
            this.Dispose();
        }

        private void OnConnectedClientSelected(int connectionId)
        {
            this.ClientOptionsUIController.gameObject.SetActive(true);
            this.CoroutineUtils.WaitUntil(
                () =>
                    {
                        var username = ServerNetworkManager.Instance.GetClientUsername(connectionId);
                        return !string.IsNullOrEmpty(username);
                    },
                () =>
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

        private void OnMainPlayerDisconnected(object sender, ClientConnectionIdEventArgs args)
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
            //TODO:
            return;

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

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            var command = NetworkCommandData.From<GameDataMarkCommand>();
            var mainPlayerConnectionId = this.MainPlayerData.ConnectionId;
            command.AddOption("Mark", args.Mark.ToString());

            ServerNetworkManager.Instance.SendClientCommand(mainPlayerConnectionId, command);
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

        private void OnClientConnected(object sender, ClientConnectionIdEventArgs args)
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
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
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
            this.GameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.GameDataQuestionsSender.OnSentQuestion += this.OnSentQuestion;
            this.GameDataQuestionsSender.OnBeforeSend += this.OnBeforeSendQuestion;

            ServerNetworkManager.Instance.OnClientConnected += this.OnClientConnected;

            this.askPlayerQuestionRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();
            this.audiencePollRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();

            this.ConnectedClientsUIController.OnSelectedPlayer += (sender, args) => this.OnConnectedClientSelected(args.ConnectionId);
        }

        private void InitializeCommands()
        {
            AvailableCategoriesCommandsInitializer.Initialize(
                ServerNetworkManager.Instance,
                this.gameDataExtractor,
                this.leaderboardDataManipulator,
                new LocalCategoriesReader());

            var selectedAnswerCommand = new SelectedAnswerCommand(this.OnReceivedSelectedAnswer);
            var selectedAskPlayerQuestionCommand = new SelectedAskPlayerQuestionCommand(ServerNetworkManager.Instance, this.MainPlayerData, this.askPlayerQuestionRouter, 60);
            var selectedAudiencePollCommand = new SelectedHelpFromAudienceJokerCommand(this.MainPlayerData, this.audiencePollRouter, 60);
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
            var commandData = NetworkCommandData.From<GameEndCommand>();
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