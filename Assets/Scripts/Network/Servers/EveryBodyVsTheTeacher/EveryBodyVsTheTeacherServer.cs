namespace Network.Servers.EveryBodyVsTheTeacher
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Commands;
    using Commands.Server;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Localization;

    using Notifications;

    using StateMachine;

    using States.EveryBodyVsTheTeacher.Server;

    using UnityEngine;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class EveryBodyVsTheTeacherServer : ExtendedMonoBehaviour, IEveryBodyVsTheTeacherServer
    {
        public const int MinMainPlayersNeededToStartGame = 6;
        public const int MaxMainPlayersNeededToStartGame = 10;

        public event EventHandler OnGameOver = delegate
            {
            };

        [Inject]
        private ICreatedGameInfoSender sender;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion currentQuestionVoteResultCollector;

        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;
    
        [Inject]
        private FirstRoundState firstRoundState;
        
        [Inject]
        private SecondRoundState secondRoundState;

        [Inject]
        private ThirdRoundState thirdRoundState;
            
        [Inject]
        private JokersData jokers;

        private readonly StateMachine stateMachine = new StateMachine();

        private HashSet<int> mainPlayersConnectionIds = new HashSet<int>();

        public bool IsGameOver
        {
            get;
            private set;
        }

        public IEnumerable<int> MainPlayersConnectionIds
        {
            get
            {
                return this.mainPlayersConnectionIds;
            }
        }

        public bool StartedGame
        {
            get;
            private set;
        }

        public int PresenterId
        {
            get;
            private set;
        }

        void Start()
        {
            this.networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(this.OnMainPlayerConnecting));
            this.playersConnectingToTheServerState.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;
            
            this.currentQuestionVoteResultCollector.OnCollectedVote += this.OnCollectedVoteForCurrentQuestion;
            this.currentQuestionVoteResultCollector.OnNoVotesCollected += this.OnNoVotesCollected;
            this.currentQuestionVoteResultCollector.OnLoadingCurrentQuestionError += this.OnLoadingQuestionError;
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.stateMachine.CurrentState == this.playersConnectingToTheServerState ||
                this.mainPlayersConnectionIds.Contains(connectionId))
            {
                return;
            }

            this.networkManager.KickPlayer(connectionId);
        }

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs eventArgs)
        {
            this.stateMachine.SetCurrentState(this.firstRoundState);

            this.currentQuestionVoteResultCollector.StartCollecting();
        }
        
        private void OnNoVotesCollected(object sender, EventArgs args)
        {
            this.EndGame();
        }

        private void OnCollectedVoteForCurrentQuestion(object sender, AnswerEventArgs args)
        {
            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                {
                    if (args.Answer == question.CorrectAnswer)
                    {
                        this.LoadNextQuestion();
                    }
                    else if (this.stateMachine.CurrentState == this.firstRoundState)
                    {
                        this.stateMachine.SetCurrentState(this.secondRoundState);
                    }
                    else if (this.stateMachine.CurrentState == this.secondRoundState)
                    {
                        this.stateMachine.SetCurrentState(this.thirdRoundState);
                    }
                    else
                    {
                        this.EndGame();
                    }
                },
                (error) =>
                {
                    this.OnLoadingQuestionError(this, new UnhandledExceptionEventArgs(error, true));
                });
        }

        private void OnLoadingQuestionError(object sender, UnhandledExceptionEventArgs args)
        {
            var message = LanguagesManager.Instance.GetValue("GameDataIterator/CantLoadCurrentQuestion");
            NotificationsController.Instance.AddNotification(Color.red, message);

            var command = new NetworkCommandData("CantLoadQuestion");
            this.networkManager.SendClientCommand(this.PresenterId, command);
        }

        private void LoadNextQuestion()
        {
            //HACK: set current question = next question. 
            this.gameDataIterator.GetNextQuestion(
                (question) =>
                {
                    this.currentQuestionVoteResultCollector.StartCollecting();
                },
                (error) =>
                {
                    OnLoadingQuestionError(this, new UnhandledExceptionEventArgs(error, true));
                });
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }
    }

}