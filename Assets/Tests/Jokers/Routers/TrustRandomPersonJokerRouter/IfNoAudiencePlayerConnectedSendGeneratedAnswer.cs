using Interfaces.Network;
using Zenject;
using Tests.DummyObjects;
using System.Linq;
using Interfaces.Network.NetworkManager;
using Assets.Tests.Extensions;
using Commands;
using Interfaces.Network.Jokers.Routers;
using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;
using Assets.Scripts.Commands.Jokers.Result;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using Utils.Unity;

namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter 
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IfNoAudiencePlayerConnectedSendGeneratedAnswer : ExtendedMonoBehaviour 
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ITrustRandomPersonJokerRouter router;

        void Start () 
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            var mainPlayersConnectionIds = Enumerable.Range(1, 10).ToArray();
            dummyServer.ConnectedMainPlayersConnectionIds = mainPlayersConnectionIds;
            dummyServer.MainPlayersConnectionIds = mainPlayersConnectionIds;

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            for (int i = 0; i < mainPlayersConnectionIds.Length; i++)
            {
                var connectionId = mainPlayersConnectionIds[i];
                dummyNetworkManager.SimulateClientConnected(connectionId, "Username " + connectionId.ToString());              
            }

            dummyNetworkManager.OnSentDataToClient += 
                (object sender, EventArgs.DataSentEventArgs args) => 
                {
                    var commandData = NetworkCommandData.Parse(args.Message);
                    if (commandData.Name == typeof(TrustRandomPersonJokerResultCommand).Name.Replace("Command", ""))
                    {
                        IntegrationTest.Pass();
                    }
                };

            router.Activate();

            this.CoroutineUtils.WaitForSeconds(1, () =>
                {
                    IntegrationTest.Fail();
                });
        }
    }
}