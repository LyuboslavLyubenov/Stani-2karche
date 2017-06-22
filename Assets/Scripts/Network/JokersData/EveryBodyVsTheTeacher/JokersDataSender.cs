using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokerTypeEventArgs = EventArgs.JokerTypeEventArgs;
using Network_JokersData = Network.JokersData;
using Network_JokersDataSender = Network.JokersDataSender;

namespace Assets.Scripts.Network.JokersData.EveryBodyVsTheTeacher
{

    using System;

    public class JokersDataSender : Network_JokersDataSender
    {
        private readonly IEveryBodyVsTheTeacherServer server;

        public JokersDataSender(Network_JokersData jokersData, IServerNetworkManager networkManager, IEveryBodyVsTheTeacherServer server)
            : base(jokersData, networkManager)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.server = server;
        }

        private void UpdateReceiverConnectionId()
        {
            base.receiverConnectionId = this.server.PresenterId;
        }

        protected override void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            this.UpdateReceiverConnectionId();
            base.OnAddedJoker(sender, args);
        }

        protected override void OnRemovedJoker(object sender, JokerTypeEventArgs args)
        {
            this.UpdateReceiverConnectionId();
            base.OnRemovedJoker(sender, args);
        }
    }
}
