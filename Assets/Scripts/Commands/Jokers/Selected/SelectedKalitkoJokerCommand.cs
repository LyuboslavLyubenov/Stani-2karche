namespace Commands.Jokers.Selected
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using EventArgs;

    using Extensions;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Utils;

    public class SelectedKalitkoJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public const int MinTimeTimeoutInSeconds = 5;

        public event EventHandler OnAllPlayersSelected = delegate
        {
        };

        public event EventHandler<ClientConnectionIdEventArgs> OnPlayerSelected = delegate
            { };

        public event EventHandler OnSelectTimeout = delegate
            { };

        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly int selectThisJokerTimeoutInSeconds;
        private readonly IList<int> playersSelectedJoker = new List<int>();
        private readonly Timer selectThisJokerTimeoutTimer;
        private bool startedSelecting = false;

        public EventHandler OnExecuted
        {
            get; set;
        }
        
        public SelectedKalitkoJokerCommand(
            IEveryBodyVsTheTeacherServer server, 
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }
            
            if (selectThisJokerTimeoutInSeconds < MinTimeTimeoutInSeconds)
            {
                throw new ArgumentOutOfRangeException("selectThisJokerTimeoutInSeconds");
            }

            this.server = server;
            this.selectThisJokerTimeoutTimer = TimerUtils.ExecuteAfter(selectThisJokerTimeoutInSeconds, this.SelectThisJokerTimeout);

            ((IExtendedTimer)this.selectThisJokerTimeoutTimer).RunOnUnityThread = true;
            this.selectThisJokerTimeoutTimer.Stop();
        }

        private void SelectThisJokerTimeout()
        {
            this.selectThisJokerTimeoutTimer.Stop();
            this.startedSelecting = false;
            this.OnSelectTimeout(this, EventArgs.Empty);
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (!this.server.StartedGame)
            {
                return;
            }

            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();

            if (!this.server.MainPlayersConnectionIds.Contains(connectionId) || 
                this.playersSelectedJoker.Contains(connectionId))
            {
                return;
            }

            this.playersSelectedJoker.Add(connectionId);
            this.OnPlayerSelected(this, new ClientConnectionIdEventArgs(connectionId));
            
            if (!this.startedSelecting)
            {
                this.selectThisJokerTimeoutTimer.Start();
                this.startedSelecting = true;
                return;
            }

            if (this.server.MainPlayersConnectionIds.All(this.playersSelectedJoker.Contains))
            {
                this.selectThisJokerTimeoutTimer.Stop();
                this.startedSelecting = false;
                
                this.OnAllPlayersSelected(this, EventArgs.Empty);
                return;
            }   
        }
    }
}