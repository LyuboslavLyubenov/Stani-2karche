﻿using IDisableRandomAnswersRouter = Interfaces.Network.Jokers.Routers.IDisableRandomAnswersRouter;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using Network;
using Jokers;

namespace Commands.Jokers.Selected
{
    using System;

    public class SelectedConsultWithTeacherJokerCommand : SelectedElectionJokerCommand
    {
        private readonly IDisableRandomAnswersRouter jokerRouter;
        
        public SelectedConsultWithTeacherJokerCommand(
            IEveryBodyVsTheTeacherServer server,
            JokersData jokersData,
            IDisableRandomAnswersRouter jokerRouter,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, jokersData, typeof(ConsultWithTheTeacherJoker), selectThisJokerTimeoutInSeconds)
        {
            if (jokerRouter == null)
            {
                throw new ArgumentNullException();
            }

            this.jokerRouter = jokerRouter;
        }

        protected override void ActivateRouter()
        {
            this.jokerRouter.Activate(2, this.server.PresenterId);
        }
    }
}