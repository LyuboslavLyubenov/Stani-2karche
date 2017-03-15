using UnityEngine;

namespace Assets.Tests.Jokers.Routers.AskClientQuestionRouter
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Zenject.Source.Usage;

    public class TimeToAnswerInSecondsMustBeAboveMinimumValue : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionRouter askClientQuestionRouter;

        void Start()
        {
            this.askClientQuestionRouter.Activate(1, 0, this.question);
        }
    }
}
