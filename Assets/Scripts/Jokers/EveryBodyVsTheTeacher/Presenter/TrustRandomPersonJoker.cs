using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter
{

    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class TrustRandomPersonJoker : IJoker
    {
        public event EventHandler OnActivated = delegate {};
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate {};
        public event EventHandler OnFinishedExecution = delegate {};

        public Sprite Image { get; private set; }
        
        public bool Activated { get; private set; }

        public TrustRandomPersonJoker(
            IClientNetworkManager networkManager,
            GameObject loadingUI,
            GameObject secondsRemainingUI,
            ISecondsRemainingUIController secondsRemainingUiController,
            GameObject notReceivedAnswerUI,
            GameObject playerAnswerUI,
            IPlayerAnswerUIController playerAnswerUIController)
        {


            this.Image = Resources.Load<Sprite>("Images");
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }
    }
}