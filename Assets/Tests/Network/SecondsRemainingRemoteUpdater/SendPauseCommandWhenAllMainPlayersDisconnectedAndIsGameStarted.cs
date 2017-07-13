using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.SecondsRemainingRemoteUpdater
{
    using System.Collections;
    using System.Linq;

    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class SendPauseCommandWhenAllMainPlayersDisconnectedAndIsGameStarted : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;
        
        [Inject]
        private ISecondsRemainingUICommandsSender secondsRemainingUiCommandsSender;

        void Start()
        {
            this.StartCoroutine(this.TestCoroutine());
        }

        private IEnumerator TestCoroutine()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            var mainPlayersConnectionIds = new int[] { 1, 2, 3 };
            dummyServer.ConnectedMainPlayersConnectionIds = mainPlayersConnectionIds;

            yield return null;

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<PauseSecondsRemainingCommand>())
                    {
                        IntegrationTest.Pass();
                    }
                };

            for (int i = 0; i < mainPlayersConnectionIds.Length; i++)
            {
                var connectionId = mainPlayersConnectionIds[i];
                dummyServer.ConnectedMainPlayersConnectionIds =
                    dummyServer.ConnectedMainPlayersConnectionIds.Except(
                        new int[]
                        {
                            connectionId
                        });
                dummyNetworkManager.FakeDisconnectPlayer(connectionId);

                yield return null;
            }
        }
    }
}