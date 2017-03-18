namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Assets.Scripts.Interfaces.Controllers;

    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public MainPlayersContainerUIController MainPlayersContainerUiController;
        
        public override void InstallBindings()
        {
            Container.Bind<IMainPlayersContainerUIController>()
                .FromInstance(this.MainPlayersContainerUiController);
        }
    }
}