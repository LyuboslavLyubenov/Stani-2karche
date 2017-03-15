namespace Assets.Tests.Jokers.Routers.AnswerPoll
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

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