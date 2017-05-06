using Controllers_AvailableJokersUIController = Controllers.AvailableJokersUIController;

namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Presenter
{
    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;

    using EventArgs;

    using UnityEngine;

    public class AvailableJokersUIController : Controllers_AvailableJokersUIController
    {
        private IElectionForJokersBinder jokersBinder;

        public AvailableJokersUIController(IElectionForJokersBinder jokersBinder)
        {
            if (jokersBinder == null)
            {
                throw new ArgumentNullException("jokersBinder");
            }

            this.jokersBinder = jokersBinder;
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
    }
}