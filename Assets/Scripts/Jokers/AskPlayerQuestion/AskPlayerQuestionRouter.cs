using System;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Jokers.AskPlayerQuestion
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class AskPlayerQuestionRouter : ExtendedMonoBehaviour
    {
        public const int MinTimeToAnswerInSeconds = 10;

        const float ChanceToGenerateCorrectAnswer = 0.8f;

        const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
        const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;

        public ServerNetworkManager NetworkManager;
        public LocalGameData LocalGameData;

        int timeToAnswerInSeconds;
        int senderConnectionId;
        int friendConnectionId;
        int elapsedTime;

        bool activated = false;

        public bool Activated
        {
            get
            {
                return this.activated;
            }
        }

        public EventHandler OnActivated = delegate
            {
            };

        public EventHandler OnSent = delegate
            {
            };

        public EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        void Start()
        {
            base.CoroutineUtils.RepeatEverySeconds(1f, this.UpdateTimer);
        }

        void OnReceivedFriendResponse(int connectionId, string answer)
        {
            if (connectionId != this.friendConnectionId)
            {
                this.NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(this.OnReceivedFriendResponse));
                return;
            }

            if (this.elapsedTime >= this.timeToAnswerInSeconds || !this.NetworkManager.IsConnected(this.senderConnectionId))
            {
                return;
            }

            this.SendMainPlayerAnswerResponse(connectionId, answer);
        }

        void SendMainPlayerAnswerResponse(int connectionId, string answer)
        {
            var sendFriendResponseCommand = NetworkCommandData.From<AskPlayerResponseCommand>();
            var friendUsername = this.NetworkManager.GetClientUsername(connectionId);
            sendFriendResponseCommand.AddOption("Username", friendUsername);
            sendFriendResponseCommand.AddOption("Answer", answer);

            this.NetworkManager.SendClientCommand(this.senderConnectionId, sendFriendResponseCommand);

            this.OnSent(this, EventArgs.Empty);

            this.Deactivate();
        }

        void UpdateTimer()
        {
            if (!this.Activated)
            {
                return;
            }

            this.elapsedTime++;

            if (this.elapsedTime >= this.timeToAnswerInSeconds)
            {
                this.Deactivate();

                var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
                this.NetworkManager.SendClientCommand(this.senderConnectionId, answerTimeoutCommandData);
                this.NetworkManager.SendClientCommand(this.friendConnectionId, answerTimeoutCommandData);
            }
        }

        void SendComputerGeneratedAnswer()
        {
            var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);

            this.CoroutineUtils.WaitForSeconds(secondsToWait, () =>
                {
                    this.LocalGameData.GetCurrentQuestion((question) =>
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
        }

        void SendQuestionToFriend()
        {
            this.LocalGameData.GetCurrentQuestion((question) =>
                {
                    var sendQuestionToFriend = NetworkCommandData.From<LoadQuestionCommand>(); 
                    var questionJSON = JsonUtility.ToJson(question.Serialize());
                    sendQuestionToFriend.AddOption("TimeToAnswer", this.LocalGameData.SecondsForAnswerQuestion.ToString());
                    sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);

                    this.NetworkManager.SendClientCommand(this.friendConnectionId, sendQuestionToFriend);
                    this.NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(this.OnReceivedFriendResponse));
                }, (exception) =>
                    {
                        Debug.LogException(exception);
                        this.Deactivate();
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
        }

        void SendJokerSettings(int connectionId)
        {
            var settingsCommandData = NetworkCommandData.From<HelpFromFriendJokerSettingsCommand>();
            settingsCommandData.AddOption("TimeToAnswerInSeconds", this.timeToAnswerInSeconds.ToString());
            this.NetworkManager.SendClientCommand(connectionId, settingsCommandData);
        }

        public void Activate(int timeToAnswerInSeconds, int senderConnectionId, int friendConnectionId)
        {
            if (this.Activated)
            {
                throw new InvalidOperationException("Already active");
            }

            if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
            }

            this.senderConnectionId = senderConnectionId;
            this.friendConnectionId = friendConnectionId;
            this.timeToAnswerInSeconds = this.LocalGameData.SecondsForAnswerQuestion;

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
            this.activated = true;
        }

        public void Deactivate()
        {
            this.StopAllCoroutines();

            this.timeToAnswerInSeconds = 0;
            this.senderConnectionId = 0;
            this.friendConnectionId = 0;
            this.elapsedTime = 0;

            this.activated = false;
        }
    }

}