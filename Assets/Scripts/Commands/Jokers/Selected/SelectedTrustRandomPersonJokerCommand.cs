using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using Network;
using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;

namespace Commands.Jokers.Selected
{

    using System;

    using Interfaces.Network.Jokers.Routers;

    public class SelectedTrustRandomPersonJokerCommand : SelectedElectionJokerCommand
    {
        private readonly ITrustRandomPersonJokerRouter jokerRouter;

        public SelectedTrustRandomPersonJokerCommand(
            IEveryBodyVsTheTeacherServer server, 
            JokersData jokersData,
            ITrustRandomPersonJokerRouter jokerRouter,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, jokersData, typeof(TrustRandomPersonJoker), selectThisJokerTimeoutInSeconds)
        {
            if (jokerRouter == null)
            {
                throw new ArgumentNullException("jokerRouter");
            }

            this.jokerRouter = jokerRouter;
        }

        protected override void ActivateRouter()
        {
            this.jokerRouter.Activate();
        }
    }
}