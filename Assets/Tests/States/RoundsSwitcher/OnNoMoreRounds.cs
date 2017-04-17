namespace Assets.Tests.States.RoundsSwitcher
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    public class OnNoMoreRounds : MonoBehaviour
    {

        void Start()
        {
            var rounds = new[]
                         {
                             new DummyRoundState(),
                             new DummyRoundState(),
                         };
            var roundsSwitcher = new RoundsSwitcher.Builder(new StateMachine())
                .AddRound(rounds[0])
                .AddRound(rounds[1])
                .Build();

            roundsSwitcher.OnNoMoreRounds += (sender, args) => IntegrationTest.Pass();

            for (int i = 0; i < rounds.Length; i++)
            {
                var round = rounds[i];
                round.FireOnMustGoOnNextRound();
            }
        }
    }
}