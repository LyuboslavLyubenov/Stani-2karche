using Network;
using Jokers.Kalitko;

namespace Commands.Jokers.Selected
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using Assets.Scripts.Extensions;

    using EventArgs;
    using EventArgs.Jokers;

    using Extensions;

    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.Network;

    using Utils;

    public abstract class SelectedElectionJokerCommand : IElectionJokerCommand
    {
        protected const int MinTimeTimeoutInSeconds = 10;

        public event EventHandler OnElectionStarted = delegate
            {
            }; 

        public event EventHandler<ElectionJokerResultEventArgs> OnElectionResult = delegate
            {
            };

        public event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelectedFor = delegate
            {
            };

        public event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelectedAgainst = delegate
            {
            };

        private readonly IList<int> playersVotedFor = new List<int>();
        private readonly IList<int> playersVotedAgainst = new List<int>();
        private readonly Timer selectThisJokerTimeoutTimer;

        private readonly JokersData jokersData;
        private readonly Type thisJokerType;

        private bool startedSelecting = false;

        protected readonly IEveryBodyVsTheTeacherServer server;

        protected SelectedElectionJokerCommand(
            IEveryBodyVsTheTeacherServer server,
            JokersData jokersData,
            Type thisJokerType,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
        {     
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }
           
            if (jokersData == null)
            {
                throw new ArgumentNullException("jokersData");
            }

            if (thisJokerType == null)
            {
                throw new ArgumentNullException("thisJokerType");
            }

            if (selectThisJokerTimeoutInSeconds < MinTimeTimeoutInSeconds)
            {
                throw new ArgumentOutOfRangeException("selectThisJokerTimeoutInSeconds");
            }

            this.server = server;
            this.jokersData = jokersData;
            this.thisJokerType = thisJokerType;
            this.selectThisJokerTimeoutTimer = TimerUtils.ExecuteAfter(selectThisJokerTimeoutInSeconds, this.SelectThisJokerTimeout);

            ((IExtendedTimer)this.selectThisJokerTimeoutTimer).RunOnUnityThread = true;
            this.selectThisJokerTimeoutTimer.Stop();
        }

        private void SelectThisJokerTimeout()
        {
            this.FinishElection();    
        }

        private void FinishElection()
        {
            var decision = 
                this.playersVotedFor.Count > this.playersVotedAgainst.Count
                               ? 
                               ElectionDecision.For 
                               :
                               ElectionDecision.Against;
            this.FinishElection(decision);
        }

        private void FinishElection(ElectionDecision decision)
        {
            this.selectThisJokerTimeoutTimer.Stop();
            this.playersVotedFor.Clear();
            this.playersVotedAgainst.Clear();
            this.startedSelecting = false;

            this.OnElectionResult(this, new ElectionJokerResultEventArgs(decision));

            if (decision == ElectionDecision.For)
            {
                this.ActivateRouter();
            }
        }

        /// <summary>
        /// Executed when mainplayers voted for this joker
        /// </summary>
        protected abstract void ActivateRouter();

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (!this.server.StartedGame || 
                !this.jokersData.AvailableJokers.Contains(this.thisJokerType))
            {
                return;
            }

            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();
            
            if (!this.server.ConnectedMainPlayersConnectionIds.Contains(connectionId) ||
                this.playersVotedFor.Contains(connectionId) ||
                this.playersVotedAgainst.Contains(connectionId) || 
                !commandsOptionsValues.ContainsKey("Decision"))
            {
                return;
            }

            var voteDecision = commandsOptionsValues["Decision"];

            if (voteDecision == ElectionDecision.For.ToString())
            {
                this.playersVotedFor.Add(connectionId);
                this.OnPlayerSelectedFor(this, new ClientConnectionIdEventArgs(connectionId));
            }
            else if (voteDecision == ElectionDecision.Against.ToString())
            {
                this.playersVotedAgainst.Add(connectionId);
                this.OnPlayerSelectedAgainst(this, new ClientConnectionIdEventArgs(connectionId));
            }
            else
            {
                return;
            }
            
            if (!this.startedSelecting)
            {
                this.selectThisJokerTimeoutTimer.Reset();
                this.startedSelecting = true;
                this.OnElectionStarted(this, EventArgs.Empty);
                return;
            }

            var areAllVoted = !this.server.ConnectedMainPlayersConnectionIds.Except(this.playersVotedFor)
                .Except(this.playersVotedAgainst)
                .Any();

            if (areAllVoted)
            {
                this.FinishElection();
                return;
            }
        }
    }
}