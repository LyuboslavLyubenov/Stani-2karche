namespace Assets.Tests.UI.QuestionUI
{

    using Controllers;

    using Interfaces.Controllers;

    using UnityEngine;

    using Zenject;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private QuestionUIController questionUIController;

        public override void InstallBindings()
        {
            this.Container.Bind<IQuestionUIController>()
                .FromInstance(this.questionUIController);
        }
    }
}