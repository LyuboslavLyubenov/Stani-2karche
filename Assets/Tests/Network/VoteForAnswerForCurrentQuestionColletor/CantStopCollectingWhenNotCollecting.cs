namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using Interfaces.Network;

    using UnityEngine;

    using Zenject;

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