namespace Assets.Scripts.Jokers.AskPlayerQuestion
{
    using System;
    using System.Linq;

    using UnityEngine;

    using Commands;
    using Commands.Client;
    using Commands.Jokers;
    using Commands.Server;
    using IO;

    using Network.NetworkManagers;
    using Utils;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class AskPlayerQuestionRouter : IDisposable
    {
        private readonly ServerNetworkManager networkManager;

        private readonly GameDataIterator gameDataIterator;

        public const int MinTimeToAnswerInSeconds = 10;

        private const float ChanceToGenerateCorrectAnswer = 0.8f;
        private const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
        private const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;
        
        private readonly int timeToAnswerInSeconds;
        private int senderConnectionId;
        private int friendConnectionId;
        private int elapsedTime;
        private bool active = false;

        private Timer_ExecuteMethodEverySeconds updateTimeTimer;

        public bool Active
        {
            get
            {
                return this.active;
            }
        }

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler OnSent = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public AskPlayerQuestionRouter(ServerNetworkManager networkManager, GameDataIterator gameDataIterator, int timeToAnswerInSeconds)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }
            
            if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
            }

            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
            this.timeToAnswerInSeconds = timeToAnswerInSeconds;

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
                            Debug.LogException(exception);
                            this.Deactivate();
                            this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                        });
                    });

            timer.AutoDispose = true;
            timer.RunOnUnityThread = true;
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

        public void Activate(int senderConnectionId, int friendConnectionId)
        {
            if (this.Active)
            {
                throw new InvalidOperationException("Already active");
            }
            
            this.senderConnectionId = senderConnectionId;
            this.friendConnectionId = friendConnectionId;

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
            this.updateTimeTimer.Stop();
            this.updateTimeTimer.Dispose();
            this.updateTimeTimer = null;
        }
    }

}