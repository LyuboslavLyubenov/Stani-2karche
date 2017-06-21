using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using QuestionsRemainingUIController = Controllers.QuestionsRemainingUIController;
using SecondsRemainingUIController = Controllers.SecondsRemainingUIController;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Network;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class PresenterInstaller : MonoInstaller
    {
        [SerializeField]
        private SecondsRemainingUIController secondsRemainingUIController;

        [SerializeField]
        private QuestionsRemainingUIController questionsRemainingUIController;

        [SerializeField]
        private MistakesRemainingUIController mistakesRemainingUIController;

        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .FromInstance(ClientNetworkManager.Instance)
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();

            this.Container.Bind<IRemoteStateActivator>()
                .To<RemoteStateActivator>()
                .AsSingle();

            var networkManager = ClientNetworkManager.Instance;

            networkManager.CommandsManager.AddCommand(new PauseSecondsRemainingCommand(this.secondsRemainingUIController));
            networkManager.CommandsManager.AddCommand(new ResumeSecondsRemainingCommand(this.secondsRemainingUIController));

            networkManager.CommandsManager.AddCommand(new LoadQuestionRemainingCountCommand(this.questionsRemainingUIController));
            networkManager.CommandsManager.AddCommand(new LoadMistakesRemainingCommand(this.mistakesRemainingUIController));
        }
    }
}