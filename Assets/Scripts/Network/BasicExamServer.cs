using System;

using UnityEngine;

namespace Assets.Scripts.Network
{
    using Commands;
    using Commands.Client;
    using Commands.Jokers;
    using Commands.Server;
    using Controllers;
    using EventArgs;
    using Interfaces;
    using Jokers;
    using Jokers.AskPlayerQuestion;
    using Jokers.AudienceAnswerPoll;
    using Statistics;
    using Utils;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class BasicExamServer : ExtendedMonoBehaviour
    {
        const float DefaultChanceToAddRandomJokerOnMarkChange = 0.08f;
        const int DefaultServerMaxPlayers = 40;

        public EventHandler OnGameOver = delegate
            {
            };

        public EventHandler<AnswerEventArgs> OnMainPlayerSelectedAnswer = delegate
            {
            };

        public ServerNetworkManager NetworkManager;

        public GameDataIterator GameData;
        public GameDataSender GameDataSender;

        public LeaderboardSerializer LeaderboardSerializer;

        public AskPlayerQuestionRouter AskPlayerQuestionRouter;
        public AudienceAnswerPollRouter AudiencePollRouter;
        public DisableRandomAnswersJokerRouter DisableRandomAnswersJokerRouter;
        public AddRandomJokerRouter AddRandomJokerRouter;

        public ConnectedClientsUIController ConnectedClientsUIController;
        public BasicExamClientOptionsUIController ClientOptionsUIController;

        public BasicExamStatisticsCollector StatisticsCollector;

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

        MainPlayerJokersDataSynchronizer mainPlayerJokersDataSynchronizer;

        float chanceToAddRandomJoker = DefaultChanceToAddRandomJokerOnMarkChange;

        int remainingTimeToAnswerMainQuestion = -1;
        // when using joker
        bool paused = false;
        bool playerSentAnswer = false;

        private ISimpleQuestion lastQuestion;

        void Awake()
        {
            this.LoadServerSettings();

            this.InitializeMainPlayerData();
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
                    if (!this.NetworkManager.IsRunning || this.IsGameOver || this.paused || this.remainingTimeToAnswerMainQuestion == -1)
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

        void OnConnectedClientSelected(int connectionId)
        {
            this.ClientOptionsUIController.gameObject.SetActive(true);
            this.CoroutineUtils.WaitForFrames(0, () =>
            {
                var username = this.NetworkManager.GetClientUsername(connectionId);
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

        void OnMainPlayerSurrender()
        {
            PlayerPrefs.SetString("Surrender", "true");
            this.EndGame();
        }

        void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.AskPlayerQuestionRouter.Deactivate();
            this.AudiencePollRouter.Deactivate();
        }

        void OnReceivedSelectedAnswer(int clientId, string answer)
        {
            if (this.IsGameOver ||
                !this.MainPlayerData.IsConnected ||
                clientId != this.MainPlayerData.ConnectionId ||
                this.paused)
            {
                return;
            }

            this.GameData.GetCurrentQuestion((question) =>
            {
                var isCorrect = (answer == question.Answers[question.CorrectAnswerIndex]);

                if (isCorrect)
                {
                    this.OnMainPlayerAnsweredCorrectly();
                }
                else
                {
                    this.OnMainPlayerAnsweredIncorrectly();
                }

            },
                Debug.LogException);

            this.playerSentAnswer = true;
        }

        void OnMainPlayerAnsweredCorrectly()
        {
            if (UnityEngine.Random.value < this.chanceToAddRandomJoker)
            {
                var jokers = JokerUtils.AllJokersTypes;
                this.AddRandomJokerRouter.Activate(this.MainPlayerData.ConnectionId, jokers, this.MainPlayerData.JokersData);
                //TODO send next question  
            }
        }

        void OnMainPlayerAnsweredIncorrectly()
        {
            this.CoroutineUtils.WaitForFrames(1, this.EndGame);
        }
        
        void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
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

        void OnMainPlayerSelectedJoker(object sender, EventArgs args)
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
        
        void OnLoadedGameData(object sender, EventArgs args)
        {
            this.remainingTimeToAnswerMainQuestion = this.GameData.SecondsForAnswerQuestion;
        }

        void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                this.remainingTimeToAnswerMainQuestion = this.GameData.SecondsForAnswerQuestion;
                this.playerSentAnswer = false;
            }
        }

        void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.IsGameOver)
            {
                this.SendEndGameInfo();
            }
        }

        void LoadServerSettings()
        {
            int serverMaxPlayers = DefaultServerMaxPlayers;

            if (PlayerPrefsEncryptionUtils.HasKey("ServerMaxPlayers"))
            {
                serverMaxPlayers = int.Parse(PlayerPrefsEncryptionUtils.GetString("ServerMaxPlayers"));    
            }

            this.NetworkManager.MaxConnections = serverMaxPlayers;    
        }

        void AttachEventHandlers()
        {
            this.MainPlayerData.OnDisconnected += this.OnMainPlayerDisconnected;

            this.GameData.OnLoaded += this.OnLoadedGameData;
            this.GameDataSender.OnSentQuestion += this.OnSentQuestion;
            this.GameDataSender.OnBeforeSend += this.OnBeforeSendQuestion;

            this.NetworkManager.OnClientConnected += this.OnClientConnected;

            this.AskPlayerQuestionRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();
            this.AudiencePollRouter.OnSent += (sender, args) => this.OnFinishedJokerExecution();

            this.ConnectedClientsUIController.OnSelectedPlayer += (sender, args) => this.OnConnectedClientSelected(args.ConnectionId);
        }

        void OnFinishedJokerExecution()
        {
            this.paused = false;
        }
        
        void InitializeMainPlayerData()
        {
            this.MainPlayerData = new MainPlayerData(this.NetworkManager);
            this.mainPlayerJokersDataSynchronizer = new MainPlayerJokersDataSynchronizer(this.NetworkManager, this.MainPlayerData);
        }

        void InitializeCommands()
        {
            var selectedAnswerCommand = new SelectedAnswerCommand(this.OnReceivedSelectedAnswer);
            var selectedAskPlayerQuestionCommand = new SelectedAskPlayerQuestionCommand(this.NetworkManager, this.MainPlayerData, this.AskPlayerQuestionRouter, 60);
            var selectedAudiencePollCommand = new SelectedAudiencePollCommand(this.MainPlayerData, this.AudiencePollRouter, this.NetworkManager, 60);
            var selectedFifthyFifthyChanceCommand = new SelectedDisableRandomAnswersJokerCommand(this.MainPlayerData, this.DisableRandomAnswersJokerRouter, this.NetworkManager, 2);
            var surrenderCommand = new SurrenderBasicExamOneTimeCommand(this.MainPlayerData, this.OnMainPlayerSurrender);
            var selectedJokerCommands = new INetworkOperationExecutedCallback[] { selectedAudiencePollCommand, selectedFifthyFifthyChanceCommand, selectedAskPlayerQuestionCommand };

            for (var i = 0; i < selectedJokerCommands.Length; i++)
            {
                selectedJokerCommands[i].OnExecuted += this.OnMainPlayerSelectedJoker;
            }

            this.NetworkManager.CommandsManager.AddCommand("AnswerSelected", selectedAnswerCommand);
            this.NetworkManager.CommandsManager.AddCommand(selectedAskPlayerQuestionCommand);
            this.NetworkManager.CommandsManager.AddCommand(selectedAudiencePollCommand);
            this.NetworkManager.CommandsManager.AddCommand(selectedFifthyFifthyChanceCommand);
            this.NetworkManager.CommandsManager.AddCommand("Surrender", surrenderCommand);
        }

        void Cleanup()
        {
            this.mainPlayerJokersDataSynchronizer = null;
            this.NetworkManager.CommandsManager.RemoveCommand("AnswerSelected");
            this.NetworkManager.CommandsManager.RemoveCommand<SelectedAskPlayerQuestionCommand>();
            this.NetworkManager.CommandsManager.RemoveCommand<SelectedAudiencePollCommand>();
            this.NetworkManager.CommandsManager.RemoveCommand("Surrender");
        }

        void UpdateRemainingTime()
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
        
        void SendEndGameInfo()
        {
            var commandData = NetworkCommandData.From<BasicExamGameEndCommand>();
            commandData.AddOption("Mark", this.GameData.CurrentMark.ToString());
            this.NetworkManager.SendAllClientsCommand(commandData);
        }

        void SavePlayerScoreToLeaderboard()
        {
            var mainPlayerName = this.MainPlayerData.Username;
            var playerScore = new PlayerScore(mainPlayerName, this.StatisticsCollector.PlayerScore, DateTime.Now);

            this.LeaderboardSerializer.SavePlayerScore(playerScore);
        }

        void ExportStatistics()
        {
            new BasicExamGeneralStatiticsExporter(this.StatisticsCollector).Export();
            new BasicExamGameDataStatisticsExporter(this.StatisticsCollector, this.GameData).Export();
        }

        public void EndGame()
        {
            this.IsGameOver = true;
            this.OnGameOver(this, EventArgs.Empty);

            this.SavePlayerScoreToLeaderboard();
            this.SendEndGameInfo();
            this.ExportStatistics();
            this.Cleanup();
        }
    }
}