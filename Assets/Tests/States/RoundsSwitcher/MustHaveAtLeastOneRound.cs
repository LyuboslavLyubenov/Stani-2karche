namespace Assets.Tests.States.RoundsSwitcher
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    public class MustHaveAtLeastOneRound : MonoBehaviour
    {
        void Start()
        {
            var roundsSwitcher = new RoundsSwitcher.Builder(new StateMachine()).Build();

            //If without expection -> test fails
            IntegrationTest.Fail();
        }
    }
}