using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Audience.ConnectedToServerState
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Audience;
    using Assets.Tests.Utils;

    using Controllers;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class ConnectedToServerStateInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject questionUI;
        
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            var question = new QuestionGenerator().GenerateQuestion();
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();

            this.Container.Bind<GameObject>()
                .WithId("QuestionUI")
                .FromInstance(this.questionUI);

            var questionUIController = this.questionUI.GetComponent<QuestionUIController>();
            this.Container.Bind<IQuestionUIController>()
                .FromInstance(questionUIController)
                .AsSingle();

            this.Container.Bind<IState>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = context.Container.Resolve<IClientNetworkManager>();
                            return new ConnectedToServerState(clientNetworkManager, this.questionUI);
                        })
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();

            var stateMachine = this.Container.Resolve<StateMachine>();
            var state = this.Container.Resolve<IState>();
            stateMachine.SetCurrentState(state);
        }
    }
}