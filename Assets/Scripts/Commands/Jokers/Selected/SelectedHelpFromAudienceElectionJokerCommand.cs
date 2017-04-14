using SelectedElectionJokerCommand = Commands.Jokers.Selected.SelectedElectionJokerCommand;

namespace Assets.Scripts.Commands.Jokers.Selected
{

    using System;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;

    public class SelectedHelpFromAudienceElectionJokerCommand : SelectedElectionJokerCommand
    {
        private readonly IHelpFromAudienceJokerRouter helpFromAudienceJokerRouter;

        private readonly IGameDataIterator gameDataIterator;

        public SelectedHelpFromAudienceElectionJokerCommand(
            IEveryBodyVsTheTeacherServer server, 
            IHelpFromAudienceJokerRouter helpFromAudienceJokerRouter,
            IGameDataIterator gameDataIterator,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, selectThisJokerTimeoutInSeconds)
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