namespace Assets.Tests.UI.Lobby.SelectPlayerTypeRouter
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Controllers.Lobby;
    using Assets.Zenject.Source.Install;

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