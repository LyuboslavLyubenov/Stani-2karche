using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer;
    using Assets.Tests.Utils;

    using Controllers;
    
    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;
    using UnityEngine.UI;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private Button gameStartButton;
        
        [SerializeField]
        private GameObject playingUI;

        [SerializeField]
        private GameObject questionUI;
        
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            this.Container.Bind<Button>()
                .WithId("GameStartButton")
                .FromInstance(this.gameStartButton);

            this.Container.Bind<Button>()
                .FromInstance(this.gameStartButton)
                .WhenInjectedInto<ConnectedToServerState>();

            this.Container.Bind<GameObject>()
                .WithId("PlayingUI")
                .FromInstance(this.playingUI);

            this.Container.Bind<GameObject>()
                .WithId("QuestionUI")
                .FromInstance(this.questionUI);

            var question = new QuestionGenerator().GenerateQuestion();
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            var questionUIController = this.questionUI.GetComponent<QuestionUIController>();
            this.Container.Bind<IQuestionUIController>()
                .FromInstance(questionUIController);

            this.Container.Bind<IState>()
                .To<ConnectedToServerState>()
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