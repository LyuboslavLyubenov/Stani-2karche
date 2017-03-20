namespace Tests.Network.VoteForAnswerForCurrentQuestionCollector
{

    using Assets.Scripts.Interfaces.Network;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class CantStartCollectingTwice : MonoBehaviour
    {
        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion answersCollector;

        void Start()
        {
            this.answersCollector.StartCollecting();
            this.answersCollector.StartCollecting();
        }
    }
}