using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.SecondsRemainingRemoteUpdater
{
    using Assets.Scripts.Interfaces.Network;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using StateMachine;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenGameIsStartedDontSendCommands : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;
        
        [Inject]
        private IRemoteSecondsRemainingUIUpdater remoteSecondsRemainingUIUpdater;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) => { IntegrationTest.Fail(); };

            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.MainPlayersConnectionIds = new int[] { };
            dummyNetworkManager.FakeDisconnectPlayer(1);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        dummyServer.MainPlayersConnectionIds = new int[]
                                                               {
                                                                   1,
                                                                   2,
                                                                   3
                                                               };
                        dummyNetworkManager.FakeConnectPlayer(3);
                    });
        }
    }
}