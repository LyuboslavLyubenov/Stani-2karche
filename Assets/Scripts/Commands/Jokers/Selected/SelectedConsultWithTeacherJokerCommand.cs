using ClientConnectionIdEventArgs = EventArgs.ClientConnectionIdEventArgs;
using IDisableRandomAnswersRouter = Interfaces.Network.Jokers.Routers.IDisableRandomAnswersRouter;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using SelectedElectionJokerCommand = Commands.Jokers.Selected.SelectedElectionJokerCommand;

namespace Assets.Scripts.Commands.Jokers.Selected
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Commands.Jokers.Selected;

    using EventArgs;

    public class SelectedConsultWithTeacherJokerCommand : SelectedElectionJokerCommand
    {
        private readonly IDisableRandomAnswersRouter jokerRouter;

        private readonly int disableAnswersCount;

        public SelectedConsultWithTeacherJokerCommand(
            IEveryBodyVsTheTeacherServer server,
            IDisableRandomAnswersRouter jokerRouter, 
            int disableAnswersCount,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, selectThisJokerTimeoutInSeconds)
        {
            if (jokerRouter == null)
            {
                throw new ArgumentNullException();
            }

            if (disableAnswersCount <= 0)
            {
                throw new ArgumentNullException();
            }

            this.jokerRouter = jokerRouter;
            this.disableAnswersCount = disableAnswersCount;
        }

        protected override void ActivateRouter()
        {
            this.jokerRouter.Activate(this.disableAnswersCount);
        }
    }
}