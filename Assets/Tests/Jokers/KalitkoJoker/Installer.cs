using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.KalitkoJoker
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers.Kalitko;
    using Assets.Tests.DummyObjects.UIControllers;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameObject>()
                .FromInstance(new GameObject())
                .AsSingle();

            Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            Container.Bind<IKalitkoJokerUIController>()
                .To<DummyKalitkoJokerUIContainer>()
                .AsSingle();

            Container.Bind<string>()
                .FromInstance("Answer")
                .WhenInjectedInto<ReceivedAnswer>();

            Container.Bind<IJoker>()
                .To<PresenterKalitkoJoker>();
        }
    }
}