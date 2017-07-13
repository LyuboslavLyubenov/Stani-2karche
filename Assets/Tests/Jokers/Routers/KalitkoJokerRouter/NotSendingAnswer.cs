using NetworkCommandData = Commands.NetworkCommandData;
using Routers_KalitkoJokerRouter = Jokers.Routers.KalitkoJokerRouter;

namespace Tests.Jokers.Routers.KalitkoJokerRouter
{

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class NotSendingAnswer : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IGameDataIterator gameDataIterator;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "KalitkoJokerResult" &&
                        !command.Options.ContainsKey("Answer"))
                    {
                        IntegrationTest.Pass();
                    }
                };

            var router = new Routers_KalitkoJokerRouter(this.networkManager, this.server, this.gameDataIterator, 0, 0);
            router.Activate();
        }
    }

}