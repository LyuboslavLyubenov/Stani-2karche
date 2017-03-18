namespace Jokers.Routers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Commands;
    using Commands.Jokers;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    public class VoteForAnswerJokerRouter : IVoteForAnswerJokerRouter
    {
        public event EventHandler OnActivated = delegate {};

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
        {
        };
        
        private readonly IAnswerPollRouter pollRouter;

        private readonly IEveryBodyVsTheTeacherServer server;

        private readonly IServerNetworkManager networkManager;
        private readonly IGameDataIterator gameDataIterator;
        
        public VoteForAnswerJokerRouter(
            IServerNetworkManager networkManager, 
            IGameDataIterator gameDataIterator, 
            IAnswerPollRouter answerPollRouter, 
            IEveryBodyVsTheTeacherServer server)
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

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
            this.pollRouter = answerPollRouter;
            this.server = server;

            this.pollRouter.OnVoteFinished += this.OnPollRouterVoteFinished;
        }

        private void OnPollRouterVoteFinished(object sender, VoteEventArgs voteEventArgs)
        {
            var highestRankedAnswer = voteEventArgs.AnswersVotes.OrderByDescending(av => av.Value).First().Key;
            var voteForAnswerJokerResultCommand = NetworkCommandData.From<VoteForAnswerJokerResultCommand>();
            voteForAnswerJokerResultCommand.AddOption("Answer", highestRankedAnswer);
            this.networkManager.SendClientCommand(this.server.PresenterId, voteForAnswerJokerResultCommand);
        }

        public void Activate(int timeToAnswerInSeconds, IEnumerable<int> clientsThatMustVote)
        {
            this.gameDataIterator.GetCurrentQuestion(
                (question) =>
                    {
                        try
                        {
                            this.pollRouter.Activate(timeToAnswerInSeconds, clientsThatMustVote, question);
                            this.OnActivated(this, EventArgs.Empty);
                        }
                        catch (Exception exception)
                        {
                            this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
                        }
                    },
                (error) =>
                {
                    this.OnError(this, new UnhandledExceptionEventArgs(error, true));
                }
           );
        }

        public void Dispose()
        {
            this.pollRouter.Dispose();
        }
    }
}
