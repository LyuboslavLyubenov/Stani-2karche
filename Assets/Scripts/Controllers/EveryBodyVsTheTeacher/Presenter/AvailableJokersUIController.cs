using Controllers_AvailableJokersUIController = Controllers.AvailableJokersUIController;
using JokerEventArgs = EventArgs.JokerEventArgs;

namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Presenter
{
    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers.EveryBodyVsTheTeacher.Presenter;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;

    using EventArgs;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class AvailableJokersUIController : Controllers_AvailableJokersUIController, IAvailableJokersUIController
    {
        [Inject]
        private IElectionForJokersBinder jokersBinder;

        public AvailableJokersUIController()
        {
            this.OnAddedJoker += _OnAddedJoker;
        }

        private void _OnAddedJoker(object sender, JokerEventArgs args)
        {
            this.jokersBinder.Bind(args.Joker);
            args.Joker.OnFinishedExecution += OnFinishedExecution;
        }

        private void OnFinishedExecution(object sender, EventArgs args)
        {
            var joker = (IJoker)sender;
            joker.OnFinishedExecution -= this.OnFinishedExecution;
            this.jokersBinder.Unbind(joker);
        }

        protected override void OnJokerClick(GameObject jokerObj, IJoker joker)
        {
            //remove functionality for clicking
        }

        public void Dispose()
        {
            this.jokersBinder.Dispose();
        }
    }
}