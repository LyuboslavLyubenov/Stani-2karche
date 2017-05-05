using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Network.EveryBodyVsTheTeacher
{
    using System;
    using System.Linq;

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;

    public class RoundsSwitcherEventsNotifier : IDisposable
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IRoundsSwitcher roundsSwitcher;
        private readonly int sendtoConnectionId;
        
        public RoundsSwitcherEventsNotifier(
            IServerNetworkManager networkManager, 
            int sendToConnectionId, 
            IRoundsSwitcher roundsSwitcher)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (sendToConnectionId <= 0 || !networkManager.ConnectedClientsConnectionId.Contains(sendToConnectionId))
            {
                throw new ArgumentException("Client with id " + sendToConnectionId + " is not connected");
            }

            if (roundsSwitcher == null)
            {
                throw new ArgumentNullException("roundsSwitcher");
            }

            this.networkManager = networkManager;
            this.roundsSwitcher = roundsSwitcher;
            this.sendtoConnectionId = sendToConnectionId;

            roundsSwitcher.OnSwitchedToNextRound += this.OnSwitchedToNextRound;
            roundsSwitcher.OnMustEndGame += this.OnMustEndGame;
            roundsSwitcher.OnNoMoreRounds += this.OnNoMoreRounds;
        }

        private void OnSwitchedToNextRound(object sender, EventArgs args)
        {
            this.networkManager.SendClientCommand(this.sendtoConnectionId, new NetworkCommandData("SwitchedToNextRound"));
        }

        private void OnMustEndGame(object sender, EventArgs args)
        {
            this.networkManager.SendClientCommand(this.sendtoConnectionId, new NetworkCommandData("TooManyWrongAnswers"));
        }

        private void OnNoMoreRounds(object sender, EventArgs args)
        {
            this.networkManager.SendClientCommand(this.sendtoConnectionId, new NetworkCommandData("NoMoreRounds"));
        }

        public void Dispose()
        {
            roundsSwitcher.OnSwitchedToNextRound -= this.OnSwitchedToNextRound;
            roundsSwitcher.OnMustEndGame -= this.OnMustEndGame;
            roundsSwitcher.OnNoMoreRounds -= this.OnNoMoreRounds;
        }
    }
}
