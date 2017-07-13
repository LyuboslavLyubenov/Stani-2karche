namespace Tests.Jokers.Routers.AnswerPoll
{

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Utils.Unity;

    using Zenject;

    public class CantStartWhenThereAreZeroClients : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;

        void Start()
        {
            this.answerPollRouter.Activate(
                10,
                new int[]
                {
                },
                this.simpleQuestion);
        }
    }

}