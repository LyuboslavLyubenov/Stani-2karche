﻿namespace Assets.Tests.States.RoundsSwitcher
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    public class ChangingToNextRound : MonoBehaviour
    {
        void Start()
        {
            var firstRound = new DummyRoundState();
            var roundsSwitcher = new RoundsSwitcher.Builder(new StateMachine())
                .AddRound(firstRound)
                .AddRound(new DummyRoundState())
                .Build();

            roundsSwitcher.OnSwitchedToNextRound += (sender, args) => IntegrationTest.Pass();
            firstRound.FireOnMustGoOnNextRound();
        }
    }
}