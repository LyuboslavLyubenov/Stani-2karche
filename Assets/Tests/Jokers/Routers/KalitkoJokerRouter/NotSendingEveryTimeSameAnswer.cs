using NetworkCommandData = Commands.NetworkCommandData;
using Routers_KalitkoJokerRouter = Jokers.Routers.KalitkoJokerRouter;

namespace Tests.Jokers.Routers.KalitkoJokerRouter
{

    using System.Collections.Generic;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class NotSendingEveryTimeSameAnswer : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private ISimpleQuestion currentQuestion;

        private HashSet<string> receivedAnswers = new HashSet<string>();

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);

                    if (command.Name == "KalitkoJokerResult")
                    {
                        this.receivedAnswers.Add(command.Options["Answer"]);
                    }
                };

            var router = new Routers_KalitkoJokerRouter(this.networkManager, this.server, this.gameDataIterator, 1f, 0.5f);

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
                        if (this.receivedAnswers.Count > 1)
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