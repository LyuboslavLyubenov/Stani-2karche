﻿namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

    using Interfaces.Controllers;

    using Zenject;

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