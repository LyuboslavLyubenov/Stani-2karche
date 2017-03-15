namespace Assets.Scripts.Jokers.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class HelpFromAudienceJokerRouter : IHelpFromAudienceJokerRouter
    {
        private const float MinCorrectAnswerVoteProcentage = 0.40f;
        private const float MaxCorrectAnswerVoteProcentage = 0.80f;

        private const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
        private const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;

        public event EventHandler OnBeforeSend = delegate
            {
            };

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler OnSent = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        private readonly IServerNetworkManager networkManager;

        private readonly IGameDataIterator gameDataIterator;
        
        private int senderConnectionId;

        private IAnswerPollRouter answerPollRouter;

        public bool Activated
        {
            get;
            private set;
        }

        public HelpFromAudienceJokerRouter(
            IServerNetworkManager networkManager,
            IGameDataIterator gameDataIterator, 
            IAnswerPollRouter answerPollRouter)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            if (answerPollRouter == null)
            {
                throw new ArgumentNullException("answerPollRouter");
            }

            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
            this.answerPollRouter = answerPollRouter;
            this.answerPollRouter.OnVoteFinished += this.OnAnswerPollVoteFinished;
        }

        private void OnAnswerPollVoteFinished(object sender, VoteEventArgs voteEventArgs)
        {
            this.SendMainPlayerVoteResult(voteEventArgs.AnswersVotes);
        }

        private void SendMainPlayerVoteResult(Dictionary<string, int> answersVotes)
        {
            this.OnBeforeSend(this, EventArgs.Empty);
            this.SendVoteResult(answersVotes);
        }

        private void SendVoteResult(Dictionary<string, int> answersVotes)
        {
            var voteResultCommandData = NetworkCommandData.From<AnswerPollResultCommand>();
            var answersVotesPairs = answersVotes.ToArray();

            for (int i = 0; i < answersVotesPairs.Length; i++)
            {
                var answer = answersVotesPairs[i].Key;
                var answerVoteCount = answersVotesPairs[i].Value;
                voteResultCommandData.AddOption(answer, answerVoteCount.ToString());
            }

            this.networkManager.SendClientCommand(this.senderConnectionId, voteResultCommandData);
            
            this.OnSent(this, EventArgs.Empty);
        }
       
        private Dictionary<string, int> GenerateAudienceVotes(ISimpleQuestion question)
        {
            var result = new Dictionary<string, int>();
            var correctAnswer = question.Answers[question.CorrectAnswerIndex];
            var correctAnswerChance = (int)(UnityEngine.Random.Range(MinCorrectAnswerVoteProcentage, MaxCorrectAnswerVoteProcentage) * 100);
            var wrongAnswersLeftOverChance = 100 - correctAnswerChance;

            result.Add(correctAnswer, correctAnswerChance);

            var incorrectAnswers = question.Answers.ToList();
            incorrectAnswers.Remove(correctAnswer);

            for (int i = 0; i < incorrectAnswers.Count - 1; i++)
            {
                var wrongAnswerChance = UnityEngine.Random.Range(0, wrongAnswersLeftOverChance);
                result.Add(incorrectAnswers[i], wrongAnswersLeftOverChance);
                wrongAnswersLeftOverChance -= wrongAnswerChance;
            }

            result.Add(incorrectAnswers.Last(), wrongAnswersLeftOverChance);
            return result;
        }

        private void SendGeneratedResultToMainPlayer()
        {
            var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);
            var timer = TimerUtils.ExecuteAfter(
                secondsToWait,
                () =>
                    {
                        this.gameDataIterator.GetCurrentQuestion(
                            (question) =>
                                {
                                    var audienceVoteResult = this.GenerateAudienceVotes(question);
                                    this.SendMainPlayerVoteResult(audienceVoteResult);
                                    this.Deactivate();
                                },
                            (exception) =>
                                {
                                    Debug.LogException(exception);
                                    this.Deactivate();
                                    this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                                });
                    });

            timer.AutoDispose = true;
            timer.RunOnUnityThread = true;
            timer.Start();
        }

        public void Deactivate()
        {
            this.answerPollRouter.Deactivate();
            this.senderConnectionId = 0;
            this.Activated = false;
        }

        public void Activate(int senderConnectionId, int timeToAnswerInSeconds)
        {
            if (this.Activated)
            {
                throw new InvalidOperationException("Already active");
            }
            
            if (timeToAnswerInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("timeToAnswerInSeconds");
            }

            if (senderConnectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("senderConnectionId");
            }
            
            this.senderConnectionId = senderConnectionId;

            var minClients = AskAudienceJoker.MinClientsForOnlineVote_Release;

            if (this.networkManager.ConnectedClientsCount < minClients)
            {
                var audiencePollSettingsCommand = NetworkCommandData.From<AnswerPollSettingsCommand>();
                audiencePollSettingsCommand.AddOption("TimeToAnswerInSeconds", this.senderConnectionId.ToString());

                this.SendGeneratedResultToMainPlayer();
                return;
            }
            
            this.gameDataIterator.GetCurrentQuestion((question) =>
                {
                    var audienceConnectionIds = this.networkManager.ConnectedClientsConnectionId.Where(connectionId => connectionId != senderConnectionId);
                    this.answerPollRouter.Activate(timeToAnswerInSeconds, audienceConnectionIds, question);

                    this.Activated = true;
                    this.OnActivated(this, EventArgs.Empty);
                }, (exception) =>
                    {
                        this.Deactivate();
                        this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
        }

        public void Dispose()
        {
            this.OnActivated = null;
            this.OnBeforeSend = null;
            this.OnSent = null;
            this.OnError = null;
            
            this.answerPollRouter.Dispose();
        }
    }
}