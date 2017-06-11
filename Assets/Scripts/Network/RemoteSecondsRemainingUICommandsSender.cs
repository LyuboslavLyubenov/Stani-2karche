using ClientConnectionIdEventArgs = EventArgs.ClientConnectionIdEventArgs;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Network
{
    using System;
    using System.Linq;

    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;

    public class RemoteSecondsRemainingUICommandsSender : IRemoteSecondsRemainingUIUpdater
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;

        private readonly MainPlayerConnectingCommand mainPlayerConnectingCommand;

        private bool paused = false;

        public RemoteSecondsRemainingUICommandsSender(IServerNetworkManager networkManager, IEveryBodyVsTheTeacherServer server)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.networkManager = networkManager;
            this.server = server;

            this.mainPlayerConnectingCommand = new MainPlayerConnectingCommand(this.OnMainPlayerConnecting);
            this.networkManager.CommandsManager.AddCommand(this.mainPlayerConnectingCommand);
            this.networkManager.OnClientDisconnected += this.OnClientDisconnected;
        }

        private void OnMainPlayerConnecting(int connectionId)
        {
            if (this.paused)
            {
                var resumeSecondsRemainingCommand = NetworkCommandData.From<ResumeSecondsRemainingCommand>();
                this.networkManager.SendClientCommand(this.server.PresenterId, resumeSecondsRemainingCommand);
                this.paused = false;
            }
        }

        private void OnClientDisconnected(object sender, ClientConnectionIdEventArgs args)
        {
            if (!this.server.MainPlayersConnectionIds.Any() && !this.paused)
            {
                var pauseSecondsRemainingCommand = NetworkCommandData.From<PauseSecondsRemainingCommand>();
                this.networkManager.SendClientCommand(this.server.PresenterId, pauseSecondsRemainingCommand);
                this.paused = true;
            }
        }

        public void Dispose()
        {
            if (this.networkManager.CommandsManager.Exists(this.mainPlayerConnectingCommand))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.mainPlayerConnectingCommand);
            }

            this.networkManager.OnClientDisconnected -= this.OnClientDisconnected;
        }
    }
}
