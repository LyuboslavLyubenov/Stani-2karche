namespace Tests.Jokers.Routers.AnswerPoll
{

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Utils.Unity;

    using Zenject;

    public class CantSetBelowMinimumTime : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;

        //expected exception
        void Start()
        {
            this.answerPollRouter.Activate(0, new []{ 1, 2, 3}, this.simpleQuestion);
        }
    }

}