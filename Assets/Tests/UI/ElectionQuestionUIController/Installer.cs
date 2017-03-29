
namespace Tests.UI.ElectionQuestionUIController
{

    using Assets.Scripts.Interfaces.Controllers;

    using Controllers;

    using DTOs;

    using Interfaces;

    using UnityEngine;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private ElectionQuestionUIController questionUIController;

        public override void InstallBindings()
        {
            var question = new SimpleQuestion("QuestionText", new[] { "Answer1", "Answer2", "Answer3", "Answer4" }, 0);

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();

            this.Container.Bind<IElectionQuestionUIController>()
                .FromInstance(this.questionUIController)
                .AsSingle();
        }
    }

}