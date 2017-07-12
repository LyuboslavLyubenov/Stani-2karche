using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Jokers.Election.MainPlayer;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer;
    using Assets.Tests.DummyObjects.Network;
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

        [SerializeField]
        private GameObject availableJokersUI;
        
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

            this.Container.Bind<GameObject>()
                .FromInstance(this.questionUI)
                .WhenInjectedInto<ConnectedToServerState>();

            var question = new QuestionGenerator().GenerateQuestion();
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            var questionUIController = this.questionUI.GetComponent<QuestionUIController>();
            this.Container.Bind<IQuestionUIController>()
                .FromInstance(questionUIController);
            
            this.Container.Bind<IState>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = context.Container.Resolve<IClientNetworkManager>();
                            var gameStartButton = context.Container.Resolve<Button>("GameStartButton");
                            var questionUI = context.Container.Resolve<GameObject>("QuestionUI");
                            var playingUI = context.Container.Resolve<GameObject>("PlayingUI");
                            
                            return 
                                new ConnectedToServerState(
                                    clientNetworkManager, 
                                    gameStartButton, 
                                    questionUI,
                                    playingUI,
                                    this.availableJokersUI,
                                    new DummyJokerElectionCommandsBinder());
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