using System;
using UnityEngine;
using Zenject;
using Tests.DummyObjects;
using Interfaces.Network.NetworkManager;
using Jokers;
using Assets.Scripts.Interfaces;
using Interfaces.Controllers;
using Assets.Tests.DummyObjects.UIControllers;
using Assets.Tests.Utils;
using Interfaces;

namespace Tests.Jokers.DisableRandomAnswers
{

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance)
                .AsSingle();

            var dummyQuestionUIController = new DummyQuestionUIController();
            var question = new QuestionGenerator().GenerateQuestion();
            dummyQuestionUIController.LoadQuestion(question); 

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();

            this.Container.Bind<IQuestionUIController>()
                .FromInstance(dummyQuestionUIController)
                .AsSingle();
            
            this.Container.Bind<IJoker>()
                .To<DisableRandomAnswersJoker>()
                .AsTransient();
        }   
    }
}
