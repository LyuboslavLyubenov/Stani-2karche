namespace Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

    using Interfaces.Controllers;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public AudiencePlayersContainerUIController AudiencePlayersContainerUi;

        public override void InstallBindings()
        {
            Container.Bind<IAudiencePlayersContainerUIController>()
                .FromInstance(this.AudiencePlayersContainerUi);
        }
    }
}
