using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IHelpFromAudienceJokerRouter = Interfaces.Network.Jokers.Routers.IHelpFromAudienceJokerRouter;
using SelectedElectionJokerCommand = Commands.Jokers.Selected.SelectedElectionJokerCommand;
using Network;
using Jokers;

namespace Assets.Scripts.Commands.Jokers.Selected
{
    using System;

    public class SelectedHelpFromAudienceElectionJokerCommand : SelectedElectionJokerCommand
    {
        private readonly IHelpFromAudienceJokerRouter helpFromAudienceJokerRouter;

        private readonly IGameDataIterator gameDataIterator;

        public SelectedHelpFromAudienceElectionJokerCommand(
            IEveryBodyVsTheTeacherServer server,
            JokersData jokersData,
            IHelpFromAudienceJokerRouter helpFromAudienceJokerRouter,
            IGameDataIterator gameDataIterator,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, jokersData, typeof(AskAudienceJoker), selectThisJokerTimeoutInSeconds)
        {
            if (helpFromAudienceJokerRouter == null)
            {
                throw new ArgumentNullException("helpFromAudienceJokerRouter");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            this.helpFromAudienceJokerRouter = helpFromAudienceJokerRouter;
            this.gameDataIterator = gameDataIterator;
        }

        protected override void ActivateRouter()
        {
            this.helpFromAudienceJokerRouter.Activate(
                this.server.PresenterId,
                this.gameDataIterator.SecondsForAnswerQuestion);
        }
    }
}