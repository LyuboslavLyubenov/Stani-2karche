using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;

namespace Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter
{

    using System;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class ConsultWithTeacherJoker : IJoker
    {
        private readonly IClientNetworkManager networkManager;

        private readonly GameObject loadingUi;

        private readonly GameObject electionQuestionUi;

        private readonly IElectionQuestionUIController electionQuestionUiController;

        public Sprite Image { get; private set; }

        public event EventHandler OnActivated;

        public event EventHandler<UnhandledExceptionEventArgs> OnError;

        public event EventHandler OnFinishedExecution;

        public bool Activated { get; private set; }


        public ConsultWithTeacherJoker(
            IClientNetworkManager networkManager, 
            GameObject loadingUI,
            GameObject electionQuestionUI,
            IElectionQuestionUIController electionQuestionUIController)
        {
            this.networkManager = networkManager;
            this.loadingUi = loadingUI;
            this.electionQuestionUi = electionQuestionUI;
            this.electionQuestionUiController = electionQuestionUIController;
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }
    }
}