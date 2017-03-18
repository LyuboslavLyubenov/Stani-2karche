namespace Jokers.Routers
{

    using System;
    using System.Linq;

    using Commands;
    using Commands.Client;
    using Commands.Jokers.Settings;

    using EventArgs;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Utils;

    public class AskPlayerQuestionJokerRouter : IAskPlayerQuestionJokerRouter
    {
        public const int MinTimeToAnswerInSeconds = 5;

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler OnSent = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public bool Active
        {
            get;
            private set;
        }

        private readonly IServerNetworkManager networkManager;

        private readonly IGameDataIterator gameDataIterator;

        private readonly IAskClientQuestionRouter askClientQuestionRouter;

        private const float ChanceToGenerateCorrectAnswer = 0.8f;
        private const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
        private const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;
        
        private int senderConnectionId;

        public AskPlayerQuestionJokerRouter(
            IServerNetworkManager networkManager,
            IGameDataIterator gameDataIterator,
            IAskClientQuestionRouter askClientQuestionRouter)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            if (askClientQuestionRouter == null)
            {
                throw new ArgumentNullException("askClientQuestionRouter");
            }

            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
            this.askClientQuestionRouter = askClientQuestionRouter;

            this.askClientQuestionRouter.OnReceivedAnswer += this.OnReceivedFriendResponse;
        }

        private void OnReceivedFriendResponse(object sender, AnswerEventArgs args)
        {
            this.SendMainPlayerAnswerResponse(this.senderConnectionId, args.Answer);
        }

        private void SendMainPlayerAnswerResponse(int connectionId, string answer)
        {
            var sendFriendResponseCommand = NetworkCommandData.From<AskClientQuestionResponseCommand>();
            var friendUsername = this.networkManager.GetClientUsername(connectionId);

            sendFriendResponseCommand.AddOption("Username", friendUsername);
            sendFriendResponseCommand.AddOption("Answer", answer);

            this.networkManager.SendClientCommand(this.senderConnectionId, sendFriendResponseCommand);

            this.Deactivate();

            this.OnSent(this, EventArgs.Empty);
        }

        private void SendComputerGeneratedAnswer(ISimpleQuestion question)
        {
            var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);
            var timer = TimerUtils.ExecuteAfter(
                secondsToWait,
                () =>
                    {
                        var shouldSendCorrect = UnityEngine.Random.value <= ChanceToGenerateCorrectAnswer;
                        var correctAnswer = question.Answers[question.CorrectAnswerIndex];

                        if (shouldSendCorrect)
                        {
                            this.SendMainPlayerAnswerResponse(NetworkCommandData.CODE_Option_ClientConnectionId_AI, correctAnswer);
                        }
                        else
                        {
                            var rndWrongAnswer = question.Answers.Where(a => a != correctAnswer).ToArray().GetRandomElement();
                            this.SendMainPlayerAnswerResponse(NetworkCommandData.CODE_Option_ClientConnectionId_AI, rndWrongAnswer);
                        }
                    });

            timer.AutoDispose = true;
            timer.RunOnUnityThread = true;
            timer.Start();
        }

        public void Activate(int senderConnectionId, int friendConnectionId, int timeToAnswerInSeconds)
        {
            if (this.Active)
            {
                throw new InvalidOperationException("Already active");
            }

            if (senderConnectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("senderConnectionId");
            }

            if (friendConnectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("friendConnectionId");
            }

            if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
            }
            
            this.senderConnectionId = senderConnectionId;
            
            var settingsCommandData = NetworkCommandData.From<AskClientQuestionSettingsCommand>();
            settingsCommandData.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
            this.networkManager.SendClientCommand(senderConnectionId, settingsCommandData);

            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                {
                    if (friendConnectionId == NetworkCommandData.CODE_Option_ClientConnectionId_AI)
                    {
                        this.SendComputerGeneratedAnswer(question);
                    }
                    else
                    {
                        this.askClientQuestionRouter.Activate(friendConnectionId, timeToAnswerInSeconds, question);
                    }
                }, (exception) =>
                {
                    Debug.LogException(exception);
                    this.Deactivate();
                    this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                });
            
            this.OnActivated(this, EventArgs.Empty);
            this.Active = true;
        }

        public void Deactivate()
        {
            this.askClientQuestionRouter.Deactivate();
            this.senderConnectionId = 0;
        }

        public void Dispose()
        {
            this.OnActivated = null;
            this.OnSent = null;
            this.OnError = null;
            this.askClientQuestionRouter.Dispose();
        }
    }
}