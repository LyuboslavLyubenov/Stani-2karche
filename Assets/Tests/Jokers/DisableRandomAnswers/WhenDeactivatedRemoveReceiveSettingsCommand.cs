using System;
using Zenject;
using Assets.Scripts.Interfaces;
using UnityEngine;
using Interfaces.Network.NetworkManager;
using Utils.Unity;
using Tests.DummyObjects;
using Commands;
using Commands.Jokers;
using Jokers.Routers;
using Assets.Tests.Utils;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using Interfaces;

namespace Tests.Jokers.DisableRandomAnswers
{

    public class WhenDeactivatedRemoveReceiveSettingsCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            this.CoroutineUtils.WaitForFrames(1, () =>
                {
                    var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
                    var gameDataIterator = new DummyGameDataIterator();
                    var serverNetworkManager = DummyServerNetworkManager.Instance;
                    gameDataIterator.CurrentQuestion = this.question;
                    var jokerRouter = new DisableRandomAnswersJokerRouter(serverNetworkManager, gameDataIterator);

                    serverNetworkManager.OnSentDataToClient += (object sender, EventArgs.DataSentEventArgs args) =>
                    {
                        dummyClientNetworkManager.FakeReceiveMessage(args.Message);
                    };

                    jokerRouter.Activate(2, 1);

                    this.CoroutineUtils.WaitForFrames(1, () =>
                        {
                            if (!this.networkManager.CommandsManager.Exists<DisableRandomAnswersJokerSettingsCommand>())
                            {
                                IntegrationTest.Pass();
                            }
                            else
                            {
                                IntegrationTest.Fail();
                            }
                        });
                });
        }
    }
}
