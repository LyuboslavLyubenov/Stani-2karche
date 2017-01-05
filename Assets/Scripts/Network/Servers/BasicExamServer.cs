﻿// ReSharper disable ArrangeTypeMemberModifiers
namespace Assets.Scripts.Network.Servers
{

    using System;

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
    using Jokers.AskPlayerQuestion;
    using Jokers.AudienceAnswerPoll;
    using NetworkManagers;
    using Statistics;
    using Utils;
    using Utils.Unity;

    using UnityEngine;

    using EventArgs = System.EventArgs;

    public class BasicExamServer : ExtendedMonoBehaviour, IGameServer
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

        public GameDataIterator GameDataIterator
        {
            get; private set;
        }

        public GameDataSender GameDataSender
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

        private BasicExamStatisticsCollector statisticsCollector;

        private LeaderboardSerializer leaderboardSerializer = null;

        private AskPlayerQuestionRouter askPlayerQuestionRouter = null;
        private AudienceAnswerPollRouter audiencePollRouter = null;
        private DisableRandomAnswersJokerRouter disableRandomAnswersJokerRouter = null;
        private AddRandomJokerRouter addRandomJokerRouter = null;

        void Awake()
        {
            var serverNetworkManager = ServerNetworkManager.Instance;

            this.GameDataIterator = new GameDataIterator();
            this.GameDataSender = new GameDataSender(this.GameDataIterator, serverNetworkManager);
            this.leaderboardSerializer = new LeaderboardSerializer();
            this.askPlayerQuestionRouter = new AskPlayerQuestionRouter(serverNetworkManager, this.GameDataIterator);
            this.audiencePollRouter = new AudienceAnswerPollRouter(serverNetworkManager, this.GameDataIterator);
            this.disableRandomAnswersJokerRouter = new DisableRandomAnswersJokerRouter();
            this.addRandomJokerRouter = new AddRandomJokerRouter();
            this.statisticsCollector = new BasicExamStatisticsCollector(serverNetworkManager, this, this.GameDataSender, this.GameDataIterator);
            this.MainPlayerData = new MainPlayerData(serverNetworkManager);
            this.mainPlayerJokersDataSynchronizer = new MainPlayerJokersDataSynchronizer(serverNetworkManager, this.MainPlayerData);

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
        }

        void OnApplicationQuit()
        {
            this.EndGame();
        }

        private void OnConnectedClientSelected(int connectionId)
        {
            this.ClientOptionsUIController.gameObject.SetActive(true);
            this.CoroutineUtils.WaitForFrames(0, () =>
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
                clientId != this.MainPlayerData.ConnectionId ||
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
                this.addRandomJokerRouter.Activate(this.MainPlayerData.ConnectionId, jokers, this.MainPlayerData.JokersData);
                //TODO send next question  
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
                this.lastQuestion = args.Question;
            }
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
            this.GameDataSender.OnSentQuestion += this.OnSentQuestion;
            this.GameDataSender.OnBeforeSend += this.OnBeforeSendQuestion;

            ServerNetworkManager.Instance.OnClientConnected += this.OnClientConnected;

            this.askPlayerQuestionRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();
            this.audiencePollRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();

            this.ConnectedClientsUIController.OnSelectedPlayer += (sender, args) => this.OnConnectedClientSelected(args.ConnectionId);
        }
        
        private void InitializeCommands()
        {
            var selectedAnswerCommand = new SelectedAnswerCommand(this.OnReceivedSelectedAnswer);
            var selectedAskPlayerQuestionCommand = new SelectedAskPlayerQuestionCommand(ServerNetworkManager.Instance, this.MainPlayerData, this.askPlayerQuestionRouter, 60);
            var selectedAudiencePollCommand = new SelectedAudiencePollCommand(this.MainPlayerData, this.audiencePollRouter, 60);
            var selectedFifthyFifthyChanceCommand = new SelectedDisableRandomAnswersJokerCommand(this.MainPlayerData, this.disableRandomAnswersJokerRouter, ServerNetworkManager.Instance, 2);
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

        private void Cleanup()
        {
            this.mainPlayerJokersDataSynchronizer = null;
            ServerNetworkManager.Instance.CommandsManager.RemoveCommand("AnswerSelected");
            ServerNetworkManager.Instance.CommandsManager.RemoveCommand<SelectedAskPlayerQuestionCommand>();
            ServerNetworkManager.Instance.CommandsManager.RemoveCommand<SelectedAudiencePollCommand>();
            ServerNetworkManager.Instance.CommandsManager.RemoveCommand("Surrender");
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

            this.leaderboardSerializer.SavePlayerScore(playerScore);
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
            this.Cleanup();

            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);
        }
    }
}