namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{

    using Assets.Tests.Extensions;

    using Exceptions.Jokers;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class ReceivedPlayerAnswerTimeout : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ITrustRandomPersonJokerRouter trustRandomPersonJokerRouter;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.serverNetworkManager;
            dummyServerNetworkManager.SimulateClientConnected(1, "Ivan");
            dummyServerNetworkManager.SimulateClientConnected(2, "Georgi");

            this.trustRandomPersonJokerRouter.OnError += (sender, args) =>
                {
                    if (args.ExceptionObject.GetType() == typeof(ReceiveAnswerTimeoutException))
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        this.trustRandomPersonJokerRouter.Activate();
                    });
        }
    }
}