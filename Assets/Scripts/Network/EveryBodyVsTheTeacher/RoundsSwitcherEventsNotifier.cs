using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
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
        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IRoundsSwitcher roundsSwitcher;
        
        public RoundsSwitcherEventsNotifier(
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server, 
            IRoundsSwitcher roundsSwitcher)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (roundsSwitcher == null)
            {
                throw new ArgumentNullException("roundsSwitcher");
            }

            this.networkManager = networkManager;
            this.server = server;
            this.roundsSwitcher = roundsSwitcher;

            roundsSwitcher.OnSwitchedToNextRound += this.OnSwitchedToNextRound;
            roundsSwitcher.OnMustEndGame += this.OnMustEndGame;
            roundsSwitcher.OnNoMoreRounds += this.OnNoMoreRounds;
        }

        private void OnSwitchedToNextRound(object sender, EventArgs args)
        {
            this.networkManager.SendClientCommand(this.server.PresenterId, new NetworkCommandData("SwitchedToNextRound"));
        }

        private void OnMustEndGame(object sender, EventArgs args)
        {
            this.networkManager.SendClientCommand(this.server.PresenterId, new NetworkCommandData("TooManyWrongAnswers"));
        }

        private void OnNoMoreRounds(object sender, EventArgs args)
        {
            this.networkManager.SendClientCommand(this.server.PresenterId, new NetworkCommandData("NoMoreRounds"));
        }

        public void Dispose()
        {
            roundsSwitcher.OnSwitchedToNextRound -= this.OnSwitchedToNextRound;
            roundsSwitcher.OnMustEndGame -= this.OnMustEndGame;
            roundsSwitcher.OnNoMoreRounds -= this.OnNoMoreRounds;
        }
    }
}
