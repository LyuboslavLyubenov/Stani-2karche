using System;
using UnityEngine;
using Zenject;
using Interfaces.Network.NetworkManager;
using Assets.Scripts.Interfaces;
using Utils.Unity;
using Tests.DummyObjects;
using Jokers.Routers;
using Assets.Tests.Utils;
using System.Collections.Generic;
using Interfaces.Controllers;
using Assets.Tests.DummyObjects.UIControllers;
using UnityTestTools.IntegrationTestsFramework.TestRunner;

namespace Tests.Jokers.DisableRandomAnswers
{
    public class WhenActivatedAndReceivedSettingsDisableReceivedAnswersCount : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private IQuestionUIController questionUIController;

        [Inject]
        private IJoker joker;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.serverNetworkManager;

            dummyServerNetworkManager.OnSentDataToClient += (object sender, EventArgs.DataSentEventArgs args) =>
            {
                dummyClientNetworkManager.FakeReceiveMessage(args.Message);
            };

            var dummyGameDataIterator = new DummyGameDataIterator();
            dummyGameDataIterator.CurrentQuestion = this.questionUIController.CurrentlyLoadedQuestion;
            var router = 
                new DisableRandomAnswersJokerRouter(
                    DummyServerNetworkManager.Instance,
                    dummyGameDataIterator);
            
            this.joker.Activate();

            this.CoroutineUtils.WaitForFrames(1, () =>
                {
                    var expectedAnswersCount = 2;
                    var actualHiddenAnswersCount = 0;
                    var dummyQuestionUIController = (DummyQuestionUIController)this.questionUIController;
                    dummyQuestionUIController.OnHideAnswer += (object sender, EventArgs.AnswerEventArgs args) =>
                    {
                        actualHiddenAnswersCount++;        
                    };
                    
                    router.Activate(expectedAnswersCount, 1);

                    this.CoroutineUtils.WaitForFrames(1, () =>
                        {
                            if (actualHiddenAnswersCount == expectedAnswersCount)
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

