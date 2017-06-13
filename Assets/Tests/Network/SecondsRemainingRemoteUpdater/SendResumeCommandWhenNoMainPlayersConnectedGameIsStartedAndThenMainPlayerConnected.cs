using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.SecondsRemainingRemoteUpdater
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class SendResumeCommandWhenNoMainPlayersConnectedGameIsStartedAndThenMainPlayerConnected : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IRemoteSecondsRemainingUIUpdater remoteSecondsRemainingUIUpdater;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            dummyServer.ConnectedMainPlayersConnectionIds = new int[] {};
            dummyNetworkManager.FakeDisconnectPlayer(1);            

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                            {
                                var command = NetworkCommandData.Parse(args.Message);
                                if (command.Name == NetworkManagerCommandUtils.GetCommandName<ResumeSecondsRemainingCommand>())
                                {
                                    IntegrationTest.Pass();
                                }
                            };

                        dummyServer.ConnectedMainPlayersConnectionIds = new int[]
                                                               {
                                                                   1
                                                               };
                        dummyNetworkManager.SimulateMainPlayerConnected(1, "Main player 1");
                    });
        }
    }

}