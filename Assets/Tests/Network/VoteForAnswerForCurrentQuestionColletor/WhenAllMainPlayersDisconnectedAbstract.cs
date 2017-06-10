using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using System.Linq;

    using Assets.Tests.Extensions;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class WhenAllMainPlayersDisconnectedAbstract : MonoBehaviour
    {
        [Inject]
        protected IServerNetworkManager networkManager;

        [Inject]
        protected IEveryBodyVsTheTeacherServer server;

        [Inject]
        protected ICollectVoteResultForAnswerForCurrentQuestion voteResultCollector;

        [Inject]
        protected int timeToAnswer;

        protected DummyServerNetworkManager dummyNetworkManager;
        protected DummyEveryBodyVsTheTeacherServer dummyServer;
    
        void Awake()
        {
            this.dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            this.dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
        }
        
        protected void ConfigureServerMainPlayers()
        {
            var mainPlayersConnectionIds = new int[] { 1, 2, 3, 4 };
            this.dummyServer.MainPlayersConnectionIds = mainPlayersConnectionIds;
        }

        protected void DisconnectMainPlayer(int connectionId)
        {
            var mainPlayersConnectionIds = this.dummyServer.MainPlayersConnectionIds.ToArray();
            this.dummyServer.MainPlayersConnectionIds = mainPlayersConnectionIds.Except(new[] { connectionId });
            this.dummyNetworkManager.FakeDisconnectPlayer(connectionId);
        }

        protected void ConnectMainPlayer(int connectionId)
        {
            var mainPlayersConnectionIds = this.dummyServer.MainPlayersConnectionIds.ToList();
            mainPlayersConnectionIds.Add(connectionId);
            this.dummyServer.MainPlayersConnectionIds = mainPlayersConnectionIds.AsEnumerable();
            this.dummyNetworkManager.SimulateMainPlayerConnected(connectionId, "Main player " + connectionId);
        }
    }

}