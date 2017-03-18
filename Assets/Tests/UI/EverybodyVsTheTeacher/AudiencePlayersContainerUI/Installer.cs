namespace Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using Assets.Scripts.Interfaces.Controllers;

    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

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
