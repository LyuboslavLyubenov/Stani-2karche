using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.UI.Jokers.MainPlayer.Election
{

    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using Zenject;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private Button thumbsUpButton;

        [SerializeField]
        private Button thumbsDownButton;

        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            this.Container.Bind<Button>()
                .WithId("ThumbsUpButton")
                .FromInstance(this.thumbsUpButton);

            this.Container.Bind<Button>()
                .WithId("ThumbsDownButton")
                .FromInstance(this.thumbsDownButton);
        }
    }
}