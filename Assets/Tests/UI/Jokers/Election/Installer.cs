﻿namespace Tests.UI.Jokers.Election
{

    using Assets.Scripts.Interfaces.Controllers.Jokers;

    using Controllers.EveryBodyVsTheTeacher.Jokers.Election;

    using UnityEngine;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private GameObject JokerElectionUI;

        public override void InstallBindings()
        {
            var controller = this.JokerElectionUI.GetComponent<JokerElectionUIController>();

            Container.Bind<IJokerElectionUIController>()
                .FromInstance(controller)
                .AsSingle();
        }
    }
}