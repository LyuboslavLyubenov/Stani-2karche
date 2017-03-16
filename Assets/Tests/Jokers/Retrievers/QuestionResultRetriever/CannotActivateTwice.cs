namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{

    using Assets.Scripts.Interfaces;
    using Assets.Zenject.Source.Usage;

    using Interfaces.Network.Jokers;

    using UnityEngine;

    public class CannotActivateTwice : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionResultRetriever askClientQuestionResultRetriever;

        void Start()
        {
            this.askClientQuestionResultRetriever.Activate(1);
            this.askClientQuestionResultRetriever.Activate(1);
        }
        
        void OnDisable()
        {
            this.askClientQuestionResultRetriever.Dispose();
        }
    }

}