namespace Tests.Jokers.Routers.AskClientQuestionRouter
{

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;

    using UnityEngine;

    using Zenject;

    public class CantActivateTwice : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionRouter askClientQuestionRouter;

        void Start()
        {
            this.askClientQuestionRouter.Activate(1, 10, this.question);
            this.askClientQuestionRouter.Activate(1, 10, this.question);
        }

    }
}
