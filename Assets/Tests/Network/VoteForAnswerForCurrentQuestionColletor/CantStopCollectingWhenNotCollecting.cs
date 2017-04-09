namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using Interfaces.Network;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class CantStopCollectingWhenNotCollecting : MonoBehaviour
    {
        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion answersCollector;

        void Start()
        {
            this.answersCollector.StopCollecting();
        }
    }

}