namespace Commands.Jokers.Selected
{

    using System;

    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;

    public class SelectedLittleIsBetterThanNothingJokerCommand : SelectedElectionJokerCommand
    {
        private readonly IDisableRandomAnswersRouter disableRandomAnswersRouter;

        public SelectedLittleIsBetterThanNothingJokerCommand(
            IEveryBodyVsTheTeacherServer server, 
            IDisableRandomAnswersRouter disableRandomAnswersRouter,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, selectThisJokerTimeoutInSeconds)
        {
            if (disableRandomAnswersRouter == null)
            {
                throw new ArgumentNullException("disableRandomAnswersRouter");
            }

            this.disableRandomAnswersRouter = disableRandomAnswersRouter;
        }

        protected override void ActivateRouter()
        {
            this.disableRandomAnswersRouter.Activate(1, this.server.PresenterId);
        }
    }
}