namespace Tests.Jokers.Routers.AskClientQuestionRouter
{

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class ClientConnectionIdMustBePositiveNumber : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionRouter askClientQuestionRouter;

        void Start()
        {
            this.askClientQuestionRouter.Activate(-1, 10, this.question);
        }
        
    }
}
