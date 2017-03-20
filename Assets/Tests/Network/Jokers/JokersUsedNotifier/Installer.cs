﻿using JokersData = Network.JokersData;
using Network_JokersUsedNotifier = Network.JokersUsedNotifier;

namespace Tests.Network.Jokers.JokersUsedNotifier
{

    using System;

    using Assets.Scripts.Interfaces.Network;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            Container.Bind<JokersData>()
                .AsSingle();

            Container.Bind<IJokersUsedNotifier>()
                .To<Network_JokersUsedNotifier>()
                .AsSingle();
        }
    }

}