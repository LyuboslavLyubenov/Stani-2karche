using Assets.Scripts.Commands.Jokers.Result;

namespace Jokers.Routers
{
    using System;
    using System.Linq;

    using Assets.Scripts.Utils;

    using Commands;
    using Commands.Client;
    using Commands.Server;

    using EventArgs;

    using Exceptions.Jokers;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;
    
    using UnityEngine;

    using Utils;

    public class TrustRandomPersonJokerRouter : ITrustRandomPersonJokerRouter
    {
        private const float ChanceForCorrectAnswer = 0.5f;

        public event EventHandler OnActivated = delegate { };
        public event EventHandler<AnswerEventArgs> OnReceivedAnswer = delegate {};
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate { };

        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IGameDataIterator gameDataIterator;

        private int playerConnectionId;

        private INetworkManagerCommand selectedAnswerCommand;

        private Timer_ExecuteMethodAfterTime answerTimeoutTimer;

        public bool Activated
        {
            get; private set;
        }

        public TrustRandomPersonJokerRouter(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server,
            IGameDataIterator gameDataIterator
            )
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            this.networkManager = networkManager;
            this.server = server;
            this.gameDataIterator = gameDataIterator;
            this.selectedAnswerCommand = new SelectedAnswerCommand(this.OnReceivedAnswerFromPlayer);
        }

        private int GetRandomPersonConnectionId(IServerNetworkManager networkManager, IEveryBodyVsTheTeacherServer server)
        {
            var presenterConnectionId = new[] { server.PresenterId };
            var playersConnectionIds = networkManager.ConnectedClientsConnectionId.Except(presenterConnectionId)
                .Except(this.server.MainPlayersConnectionIds)
                .ToList();
            return playersConnectionIds.GetRandomElement();
        }

        private void SendPlayerQuestion(ISimpleQuestion question, int timeToAnswerInSeconds, int playerConnectionId)
        {
            var questionJSON = JsonUtility.ToJson(question.Serialize());
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", timeToAnswerInSeconds.ToString());
            this.networkManager.SendClientCommand(playerConnectionId, loadQuestionCommand);
        }
        
        private void OnReceivedAnswerFromPlayer(int connectionId, string answer)
        {
            if (this.playerConnectionId != connectionId)
            {
                return;
            }

            this.CleanUp();
            this.Activated = false;

            var username = this.networkManager.GetClientUsername(connectionId);
            this.SendAnswerToPresenter(username, answer);

            this.OnReceivedAnswer(this, new AnswerEventArgs(answer, null));
        }

        private void SendAnswerToPresenter(string username, string answer)
        {
            var commandData = NetworkCommandData.From<TrustRandomPersonJokerResultCommand>();
            commandData.AddOption("Username", username);
            commandData.AddOption("Answer", answer);
            this.networkManager.SendClientCommand(this.server.PresenterId, commandData);
        }

        private void OnReceiveAnswerTimeout()
        {
            this.CleanUp();
            this.Activated = false;
            this.OnError(this, new UnhandledExceptionEventArgs(new ReceiveAnswerTimeoutException(), true));
        }

        private void CleanUp()
        {
            if (this.networkManager.CommandsManager.Exists(this.selectedAnswerCommand))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.selectedAnswerCommand);
            }

            this.answerTimeoutTimer.Stop();
            this.answerTimeoutTimer.Dispose();
        }

        private void SendQuestionToRandomAudiencePlayer(ISimpleQuestion question)
        {
            this.playerConnectionId = this.GetRandomPersonConnectionId(this.networkManager, this.server);
            this.SendPlayerQuestion(question, this.gameDataIterator.SecondsForAnswerQuestion, this.playerConnectionId);
            this.networkManager.CommandsManager.AddCommand("AnswerSelected", this.selectedAnswerCommand);
        }

        private void SendGeneratedQuestionToPresenter(ISimpleQuestion question)
        {
            var shouldSendCorrect = new System.Random().NextDouble() <= ChanceForCorrectAnswer;
            var answer = string.Empty;

            if (shouldSendCorrect)
            {
                answer = question.CorrectAnswer;
            }
            else
            {
                answer = question.Answers.Except(new string[] { question.CorrectAnswer })
                    .ToList()
                    .GetRandomElement();
            }

            this.SendAnswerToPresenter("Компютър", answer);
        }
        
        public void Activate()
        {
            if (this.Activated)
            {
                throw new InvalidOperationException("Already activated");
            }

            if (this.networkManager.ConnectedClientsCount == 0)
            {
                throw new InvalidOperationException("Must have at least 1 connected client to server (not presenter)");
            }

            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                    {
                        var connectedMainPlayersConnectionIds = this.server.ConnectedMainPlayersConnectionIds; 
                        var areAnyAudiencePlayersConnected = 
                            connectedMainPlayersConnectionIds.Except(this.networkManager.ConnectedClientsConnectionId)
                                .Any();  

                        if (areAnyAudiencePlayersConnected)
                        {
                            this.SendQuestionToRandomAudiencePlayer(question);
                            this.answerTimeoutTimer = 
                                TimerUtils.ExecuteAfter(this.gameDataIterator.SecondsForAnswerQuestion, this.OnReceiveAnswerTimeout);
                            this.answerTimeoutTimer.AutoDispose = true;
                            this.answerTimeoutTimer.RunOnUnityThread = true;
                            this.answerTimeoutTimer.Start();
                        }
                        else
                        {
                            this.SendGeneratedQuestionToPresenter(question);
                        }
                        
                        this.Activated = true;
                        this.OnActivated(true, EventArgs.Empty);
                    },
                (exception) =>
                    {
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
        }
    }
}