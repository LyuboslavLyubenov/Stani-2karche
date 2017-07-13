using NetworkCommandData = Commands.NetworkCommandData;
using Routers_KalitkoJokerRouter = Jokers.Routers.KalitkoJokerRouter;

namespace Tests.Jokers.Routers.KalitkoJokerRouter
{

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class NotSendingEveryTimeAnswer : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private ISimpleQuestion currentQuestion;

        private int receivedAnswerCount = 0;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);

                    if (command.Name == "KalitkoJokerResult" &&
                        command.Options.ContainsKey("Answer"))
                    {
                        this.receivedAnswerCount++;
                    }
                };

            var router = new Routers_KalitkoJokerRouter(this.networkManager, this.server, this.gameDataIterator);

            for (int i = 0; i < 5; i++)
            {
                this.CoroutineUtils.WaitForSeconds(0.5f + (i * 0.5f),
                    () =>
                        {
                            router.Activate();                            
                        });
            }

            this.CoroutineUtils.WaitForSeconds(3f,
                () =>
                    {
                        if (this.receivedAnswerCount < 5)
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }
}