namespace Tests.UI.Lobby.SelectPlayerTypeRouter
{

    using Controllers;
    using Controllers.Lobby;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamSelectPlayerTypeUiController;

        public SelectPlayerTypeUIController
            EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            var selectPlayerTypeRouter = new SelectPlayerTypeRouter(this.BasicExamSelectPlayerTypeUiController, this.EveryBodyVsTheTeacherSelectPlayerTypeUiController);

            this.Container.Bind<SelectPlayerTypeRouter>()
                .FromInstance(selectPlayerTypeRouter)
                .AsSingle();
        }
    }

}