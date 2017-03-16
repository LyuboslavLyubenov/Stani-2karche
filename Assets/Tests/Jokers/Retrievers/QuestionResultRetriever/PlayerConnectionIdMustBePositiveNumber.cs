namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{

    using Assets.Scripts.Interfaces;
    using Assets.Zenject.Source.Usage;

    using Interfaces.Network.Jokers;

    using UnityEngine;
    
    public class PlayerConnectionIdMustBePositiveNumber : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionResultRetriever askClientQuestionResultRetriever;

        void Start()
        {
            this.askClientQuestionResultRetriever.Activate(0);
        }
        
        void OnDisable()
        {
            this.askClientQuestionResultRetriever.Dispose();
        }
    }

}