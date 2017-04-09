﻿using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var server = new DummyEveryBodyVsTheTeacherServer
                         {
                             StartedGame = true
                         };

            Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();
            
            Container.Bind<SelectedKalitkoJokerCommand>()
                .FromMethod(
                    (context) =>
                        {
                            var command =
                                new SelectedKalitkoJokerCommand(
                                    context.Container.Resolve<IEveryBodyVsTheTeacherServer>(),
                                    new DummyKalitkoJokerRouter());
                            return command;
                        });
        }
    }
}