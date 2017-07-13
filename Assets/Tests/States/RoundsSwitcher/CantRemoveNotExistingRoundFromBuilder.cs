namespace Assets.Tests.States.RoundsSwitcher
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    public class CantRemoveNotExistingRoundFromBuilder : MonoBehaviour
    {
        void Start()
        {
            var roundsSwitcher = new RoundsSwitcher.Builder(new StateMachine())
                .RemoveRound(new DummyRoundState())
                .Build();

            
            IntegrationTest.Fail();
        }
    }
}