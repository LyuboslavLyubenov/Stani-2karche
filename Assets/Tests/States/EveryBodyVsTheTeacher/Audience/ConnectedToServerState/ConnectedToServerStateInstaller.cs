namespace Assets.Tests.States.EveryBodyVsTheTeacher.Audience.ConnectedToServerState
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Audience;
    using Assets.Tests.Utils;

    using Controllers;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject;

    public class ConnectedToServerStateInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject questionUI;
        
        public override void InstallBindings()
        {
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

            this.Container.Bind<ConnectedToServerState>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = context.Container.Resolve<IClientNetworkManager>();
                            return new ConnectedToServerState(clientNetworkManager, this.questionUI);
                        })
                .AsSingle();
        }
    }
}