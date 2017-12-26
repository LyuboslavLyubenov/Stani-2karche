using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using QuestionsRemainingUIController = Controllers.QuestionsRemainingUIController;
using SecondsRemainingUIController = Controllers.SecondsRemainingUIController;
using ThreadUtils = Utils.ThreadUtils;
using Commands;
using Assets.Scripts.Commands.Jokers.Result;
using Assets.Scripts.Interfaces.Controllers;
using Controllers.EveryBodyVsTheTeacher.Jokers;
using Commands.Jokers;
using Controllers;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Network;

    using StateMachine;

    using UnityEngine;

    using Zenject;

    public class PresenterInstaller : MonoInstaller
    {
        [SerializeField]
        private SecondsRemainingUIController secondsRemainingUIController;

        [SerializeField]
        private QuestionsRemainingUIController questionsRemainingUIController;

        [SerializeField]
        private MistakesRemainingUIController mistakesRemainingUIController;

        [SerializeField]
        private GameObject kalitkoJokerUI;

        [SerializeField]
        private QuestionUIController questionUIController;

        [SerializeField]
        private GameObject notReceivedAnswerUI;

        //friend answer ui controller
        [SerializeField]
        private FriendAnswerUIController playerAnswerUIController;

        private void AddResultJokerCommands(CommandsManager commandsManager)
        {
            var kalitkoJokerUIController = kalitkoJokerUI.GetComponent<KalitkoJokerContainerUIController>();
            commandsManager.AddCommand(
                new KalitkoJokerResultCommand(kalitkoJokerUIController, kalitkoJokerUI));
            commandsManager.AddCommand(
                new TrustRandomPersonJokerResultCommand(
                    this.secondsRemainingUIController.gameObject,
                    this.notReceivedAnswerUI,
                    this.playerAnswerUIController.gameObject,
                    this.playerAnswerUIController));
            commandsManager.AddCommand(
                new DisableRandomAnswersJokerSettingsCommand(this.questionUIController));

        }


        public override void InstallBindings()
        {
            var threadUtils = ThreadUtils.Instance;

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
        
            this.AddResultJokerCommands(networkManager.CommandsManager);   
        }
    }
}