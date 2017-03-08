namespace Assets.Scripts.Jokers.Routers
{

    using System;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils;

    using UnityEngine;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class AskPlayerQuestionRouter : IAskPlayerQuestionRouter
    {
        public const int MinTimeToAnswerInSeconds = 10;

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
            get
            {
                return this.active;
            }
        }

        private readonly IServerNetworkManager networkManager;

        private readonly IGameDataIterator gameDataIterator;
        
        private const float ChanceToGenerateCorrectAnswer = 0.8f;
        private const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
        private const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;
        
        private int timeToAnswerInSeconds;
        private int senderConnectionId;
        private int friendConnectionId;
        private int elapsedTime;
        private bool active = false;

        private Timer_ExecuteMethodEverySeconds updateTimeTimer;

        public AskPlayerQuestionRouter(IServerNetworkManager networkManager, IGameDataIterator gameDataIterator)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }
            
            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;

            this.updateTimeTimer = TimerUtils.ExecuteEvery(1f, this.UpdateTimer);
            this.updateTimeTimer.RunOnUnityThread = true;
            this.updateTimeTimer.Start();
        }
        
        private void OnReceivedFriendResponse(int connectionId, string answer)
        {
            if (connectionId != this.friendConnectionId)
            {
                this.networkManager.CommandsManager.AddCommand("AnswerSelected", new SelectedAnswerOneTimeCommand(this.OnReceivedFriendResponse));
                return;
            }

            if (this.elapsedTime >= this.timeToAnswerInSeconds || 
                !this.networkManager.IsConnected(this.senderConnectionId))
            {
                return;
            }

            this.SendMainPlayerAnswerResponse(connectionId, answer);
        }

        private void SendMainPlayerAnswerResponse(int connectionId, string answer)
        {
            var sendFriendResponseCommand = NetworkCommandData.From<AskPlayerResponseCommand>();
            var friendUsername = this.networkManager.GetClientUsername(connectionId);

            sendFriendResponseCommand.AddOption("Username", friendUsername);
            sendFriendResponseCommand.AddOption("Answer", answer);

            this.networkManager.SendClientCommand(this.senderConnectionId, sendFriendResponseCommand);

            this.Deactivate();

            this.OnSent(this, EventArgs.Empty);
        }

        private void UpdateTimer()
        {
            if (!this.Active)
            {
                return;
            }

            this.elapsedTime++;

            if (this.elapsedTime < this.timeToAnswerInSeconds)
            {
                return;
            }
            
            this.Deactivate();

            var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
            this.networkManager.SendClientCommand(this.senderConnectionId, answerTimeoutCommandData);
            this.networkManager.SendClientCommand(this.friendConnectionId, answerTimeoutCommandData);
        }

        private void SendComputerGeneratedAnswer()
        {
            var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);
            var timer = TimerUtils.ExecuteAfter(
                secondsToWait,
                () =>
                    {
                        this.gameDataIterator.GetCurrentQuestion((question) =>
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
                        }, (exception) =>
                        {
                            this.Deactivate();
                            this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                        });
                    });

            timer.AutoDispose = true;
            timer.RunOnUnityThread = true;
            timer.Start();
        }

        private void SendQuestionToFriend()
        {
            this.gameDataIterator.GetCurrentQuestion((question) =>
                {
                    var sendQuestionToFriend = NetworkCommandData.From<LoadQuestionCommand>(); 
                    var questionJSON = JsonUtility.ToJson(question.Serialize());
                    sendQuestionToFriend.AddOption("TimeToAnswer", this.gameDataIterator.SecondsForAnswerQuestion.ToString());
                    sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);

                    this.networkManager.SendClientCommand(this.friendConnectionId, sendQuestionToFriend);
                    this.networkManager.CommandsManager.AddCommand("AnswerSelected", new SelectedAnswerOneTimeCommand(this.OnReceivedFriendResponse));
                }, (exception) =>
                    {
                        Debug.LogException(exception);
                        this.Deactivate();
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
        }

        private void SendJokerSettings(int connectionId)
        {
            var settingsCommandData = NetworkCommandData.From<HelpFromFriendJokerSettingsCommand>();
            settingsCommandData.AddOption("TimeToAnswerInSeconds", this.timeToAnswerInSeconds.ToString());
            this.networkManager.SendClientCommand(connectionId, settingsCommandData);
        }

        public void Activate(int senderConnectionId, int friendConnectionId, int timeToAnswerInSeconds)
        {
            if (this.Active)
            {
                throw new InvalidOperationException("Already active");
            }

            if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
            }
            
            this.senderConnectionId = senderConnectionId;
            this.friendConnectionId = friendConnectionId;
            this.timeToAnswerInSeconds = timeToAnswerInSeconds;

            this.SendJokerSettings(senderConnectionId);

            if (friendConnectionId == NetworkCommandData.CODE_Option_ClientConnectionId_AI)
            {
                this.SendComputerGeneratedAnswer();
            }
            else
            {
                this.SendQuestionToFriend();    
            }

            this.OnActivated(this, EventArgs.Empty);
            this.active = true;
        }

        public void Deactivate()
        {
            this.senderConnectionId = 0;
            this.friendConnectionId = 0;
            this.elapsedTime = 0;

            this.active = false;
        }

        public void Dispose()
        {
            this.OnActivated = null;
            this.OnSent = null;
            this.OnError = null;

            try
            {
                this.updateTimeTimer.Stop();
            }
            finally
            {
                this.updateTimeTimer.Dispose();
                this.updateTimeTimer = null;
            }
        }
    }
}