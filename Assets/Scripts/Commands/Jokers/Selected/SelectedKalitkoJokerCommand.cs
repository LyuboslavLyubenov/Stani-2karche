using Network;
using Jokers.EveryBodyVsTheTeacher.Kalitko;

namespace Commands.Jokers.Selected
{
    using System;

    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;

    public class SelectedKalitkoJokerCommand : SelectedElectionJokerCommand
    {
        private readonly IKalitkoJokerRouter kalitkoJokerRouter;

        public SelectedKalitkoJokerCommand(
            IEveryBodyVsTheTeacherServer server,
            JokersData jokersData,
            IKalitkoJokerRouter kalitkoJokerRouter,             
            int selectThisJokerTimeoutInSeconds = SelectedElectionJokerCommand.MinTimeTimeoutInSeconds)
            : base(server, jokersData, typeof(KalitkoJoker), selectThisJokerTimeoutInSeconds)
        {
            if (kalitkoJokerRouter == null)
            {
                throw new ArgumentNullException("kalitkoJokerRouter");
            }

            this.kalitkoJokerRouter = kalitkoJokerRouter;
        }

        protected override void ActivateRouter()
        {
            this.kalitkoJokerRouter.Activate();
        }
    }
}