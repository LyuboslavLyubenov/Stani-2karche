namespace Assets.Tests.Jokers.Routers.AnswerPoll
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

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