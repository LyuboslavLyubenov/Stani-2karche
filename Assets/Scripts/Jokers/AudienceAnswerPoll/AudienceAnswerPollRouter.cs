namespace Assets.Scripts.Jokers.AudienceAnswerPoll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Commands;
    using Commands.Client;
    using Commands.Jokers;
    using Commands.Server;
    using DTOs;
    using Interfaces;
    using IO;
    using Network.NetworkManagers;
    using Utils.Unity;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class AudienceAnswerPollRouter : ExtendedMonoBehaviour
    {
        public const int MinTimeToAnswerInSeconds = 10;

        private const float MinCorrectAnswerVoteProcentage = 0.40f;
        private const float MaxCorrectAnswerVoteProcentage = 0.80f;

        private const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
        private const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;

        public EventHandler OnBeforeSend = delegate
            {
            };

        public EventHandler OnActivated = delegate
            {
            };

        public EventHandler OnSent = delegate
            {
            };

        public EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };
    
        public ServerNetworkManager NetworkManager;
        public GameDataIterator LocalGameData;

        private int timeToAnswerInSeconds;
        private int senderConnectionId;
        private int elapsedTime;

        private List<int> clientsThatMustVote = new List<int>();
        private List<int> votedClientsConnectionId = new List<int>();

        private Dictionary<string, int> answersVotes = new Dictionary<string, int>();
        
        public bool Activated
        {
            get;
            private set;
        }
        
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.CoroutineUtils.RepeatEverySeconds(1f, this.UpdateTimer);
            this.NetworkManager.CommandsManager.AddCommand("AnswerSelected", new SelectedAnswerCommand(this.OnReceivedVote));
        }
        
        private void OnReceivedVote(int connectionId, string answer)
        {
            if (!this.Activated)
            {
                return;
            }

            if (!this.clientsThatMustVote.Contains(connectionId))
            {
                return;
            }

            this.answersVotes[answer]++;
            this.votedClientsConnectionId.Add(connectionId);

            if (this.AreFinishedVoting())
            {
                this.NoMoreTimeToAnswer();
                this.SendMainPlayerVoteResult();
                return;
            }
        }

        private void UpdateTimer()
        {
            if (!this.Activated)
            {
                return;
            }

            this.elapsedTime++;

            if (!this.AreFinishedVoting())
            {
                return;
            }
            
            this.TellClientsThatJokerIsDeactivated();
            this.SendMainPlayerVoteResult();
        }

        private void NoMoreTimeToAnswer()
        {
            var answerTimeoutCommandData = NetworkCommandData.From<AnswerTimeoutCommand>();
            this.NetworkManager.SendAllClientsCommand(answerTimeoutCommandData, this.senderConnectionId);
        }

        private void SendMainPlayerVoteResult()
        {
            this.OnBeforeSend(this, EventArgs.Empty);
            this.SendVoteResult();
        }

        private void SendVoteResult()
        {
            var voteResultCommandData = NetworkCommandData.From<AudiencePollResultCommand>();
            var answersVotesPairs = this.answersVotes.ToArray();

            for (int i = 0; i < answersVotesPairs.Length; i++)
            {
                var answer = answersVotesPairs[i].Key;
                var answerVoteCount = answersVotesPairs[i].Value;
                voteResultCommandData.AddOption(answer, answerVoteCount.ToString());
            }

            this.NetworkManager.SendClientCommand(this.senderConnectionId, voteResultCommandData);

            this.Deactivate();

            this.OnSent(this, EventArgs.Empty);
        }


        private bool AreFinishedVoting()
        {
            if (this.elapsedTime >= this.timeToAnswerInSeconds)
            {
                return true;
            }

            for (int i = 0; i < this.clientsThatMustVote.Count; i++)
            {
                var connectionId = this.clientsThatMustVote[i];
            
                if (!this.votedClientsConnectionId.Contains(connectionId))
                {
                    return false;
                }
            }

            return true;
        }

        private void SendJokerSettings()
        {
            var setAskAudienceJokerSettingsCommand = NetworkCommandData.From<AudiencePollSettingsCommand>();
            setAskAudienceJokerSettingsCommand.AddOption("TimeToAnswerInSeconds", this.timeToAnswerInSeconds.ToString());
            this.NetworkManager.SendClientCommand(this.senderConnectionId, setAskAudienceJokerSettingsCommand);
        }

        private void SendQuestionToAudience(ISimpleQuestion question)
        {
            var sendQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(question.Serialize());
            sendQuestionCommand.AddOption("QuestionJSON", questionJSON);
            this.NetworkManager.SendAllClientsCommand(sendQuestionCommand, this.senderConnectionId);
        }

        private void TellClientsThatJokerIsDeactivated()
        {
            var clients = this.clientsThatMustVote.ToList();
            clients.Add(this.senderConnectionId);

            var notificationCommand = new NetworkCommandData("ShowNotification");
            notificationCommand.AddOption("Color", "yellow");
            notificationCommand.AddOption("Message", "Voting is over!");

            for (int i = 0; i < clients.Count; i++)
            {
                var connectionId = clients[i];
                this.NetworkManager.SendClientCommand(connectionId, notificationCommand);
            }
        }

        private void ResetAnswerVotes(ISimpleQuestion question)
        {
            this.answersVotes.Clear();

            for (int i = 0; i < question.Answers.Length; i++)
            {
                var answer = question.Answers[i];
                this.answersVotes.Add(answer, 0);
            }
        }

        private void GenerateAudienceVotes(ISimpleQuestion question)
        { 
            var correctAnswer = question.Answers[question.CorrectAnswerIndex]; 
            var correctAnswerChance = (int)(UnityEngine.Random.Range(MinCorrectAnswerVoteProcentage, MaxCorrectAnswerVoteProcentage) * 100); 
            var wrongAnswersLeftOverChance = 100 - correctAnswerChance; 

            this.answersVotes.Add(correctAnswer, correctAnswerChance); 

            var incorrectAnswers = question.Answers.ToList();
            incorrectAnswers.Remove(correctAnswer);

            for (int i = 0; i < incorrectAnswers.Count - 1; i++)
            { 
                var wrongAnswerChance = UnityEngine.Random.Range(0, wrongAnswersLeftOverChance); 
                this.answersVotes.Add(incorrectAnswers[i], wrongAnswersLeftOverChance); 
                wrongAnswersLeftOverChance -= wrongAnswerChance; 
            }  

            this.answersVotes.Add(incorrectAnswers.Last(), wrongAnswersLeftOverChance);
        }

        private void SendGeneratedResultToMainPlayer()
        {
            var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);
            this.CoroutineUtils.WaitForSeconds(secondsToWait, () =>
                {
                    this.LocalGameData.GetCurrentQuestion((question) =>
                        {
                            this.GenerateAudienceVotes(question);
                            this.SendMainPlayerVoteResult();
                            this.Deactivate();
                        }, (exception) =>
                            {
                                Debug.LogException(exception);
                                this.Deactivate();
                                this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                            });    
                });
        }

        public void Deactivate()
        {
            this.TellClientsThatJokerIsDeactivated();

            this.StopAllCoroutines();

            this.clientsThatMustVote.Clear();
            this.votedClientsConnectionId.Clear();
            this.answersVotes.Clear();

            this.timeToAnswerInSeconds = 0;
            this.senderConnectionId = 0;
            this.elapsedTime = -1;

            this.Activated = false;
        }

        public void Activate(int senderConnectionId, int timeToAnswerInSeconds, MainPlayerData mainPlayerData)
        {
            if (this.Activated)
            {
                throw new InvalidOperationException("Already active");
            }

            if (timeToAnswerInSeconds <= 0)
            {
                throw new ArgumentNullException();
            }

            if (mainPlayerData == null)
            {
                throw new ArgumentNullException("mainPlayerData");
            }
            
            var minClients = AskAudienceJoker.MinClientsForOnlineVote_Release;

            this.senderConnectionId = senderConnectionId;
            this.timeToAnswerInSeconds = timeToAnswerInSeconds;
            
            if (this.NetworkManager.ConnectedClientsCount < minClients)
            {
                this.answersVotes.Clear();
                this.SendJokerSettings();
                this.SendGeneratedResultToMainPlayer();
                return;
            }

            this.elapsedTime = 1;

            var audienceConnectionIds = this.NetworkManager.ConnectedClientsConnectionId.Where(connectionId => connectionId != senderConnectionId);
            this.clientsThatMustVote.AddRange(audienceConnectionIds);

            this.LocalGameData.GetCurrentQuestion((question) =>
                {
                    this.ResetAnswerVotes(question);
                    this.SendJokerSettings();
                    this.SendQuestionToAudience(question);
                    this.Activated = true;
                    this.OnActivated(this, EventArgs.Empty);
                }, (exception) =>
                    {
                        Debug.LogException(exception);
                        this.Deactivate();
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
        }
    }

}