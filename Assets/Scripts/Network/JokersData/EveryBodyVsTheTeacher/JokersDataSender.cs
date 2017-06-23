using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokerTypeEventArgs = EventArgs.JokerTypeEventArgs;
using Network_JokersData = Network.JokersData;
using Network_JokersDataSender = Network.JokersDataSender;

namespace Assets.Scripts.Network.JokersData.EveryBodyVsTheTeacher
{

    using System;
    using System.Linq;

    public class JokersDataSender : Network_JokersDataSender
    {
        private readonly IEveryBodyVsTheTeacherServer server;

        public JokersDataSender(
            Network_JokersData jokersData, 
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server)
            : base(jokersData, networkManager)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.server = server;
        }
        
        protected override void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            var connectionIds = this.server.ConnectedMainPlayersConnectionIds.ToArray();

            for (int i = 0; i < connectionIds.Length; i++)
            {
                this.receiverConnectionId = connectionIds[i];
                base.OnAddedJoker(sender, args);
            }
        }

        protected override void OnRemovedJoker(object sender, JokerTypeEventArgs args)
        {
            var connectionIds = this.server.ConnectedMainPlayersConnectionIds.ToArray();

            for (int i = 0; i < connectionIds.Length; i++)
            {
                this.receiverConnectionId = connectionIds[i];
                base.OnAddedJoker(sender, args);
            }
        }
    }
}
