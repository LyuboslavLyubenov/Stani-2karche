using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using SelectedElectionJokerCommand = Commands.Jokers.Selected.SelectedElectionJokerCommand;

namespace Assets.Scripts.Commands.Jokers.Selected
{
    using System;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    
    public class SelectedTrustRandomPersonJokerCommand : SelectedElectionJokerCommand
    {
        private readonly ITrustRandomPersonJokerRouter jokerRouter;

        public SelectedTrustRandomPersonJokerCommand(
            IEveryBodyVsTheTeacherServer server, 
            ITrustRandomPersonJokerRouter jokerRouter,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, selectThisJokerTimeoutInSeconds)
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