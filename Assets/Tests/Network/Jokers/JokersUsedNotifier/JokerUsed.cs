using JokersData = Network.JokersData;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.Jokers.JokersUsedNotifier
{

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class JokerUsed : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private JokersData jokersData;

        [Inject]
        private IJokersUsedNotifier jokersUsedNotifier;

        void Start()
        {
            var jokerType = typeof(DummyJoker);

            this.jokersUsedNotifier.OnUsedJoker += (sender, args) =>
                {
                    if (args.JokerType == jokerType)
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };

            this.jokersData.AddJoker(jokerType);
            this.CoroutineUtils.WaitForSeconds(
                0.5f,
                () =>
                    {
                        var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
                        var usedJokerCommand = new NetworkCommandData("Selected" + jokerType.Name);
                        dummyServerNetworkManager.FakeReceiveMessage(1, usedJokerCommand.ToString());
                    });
        }
    }
}