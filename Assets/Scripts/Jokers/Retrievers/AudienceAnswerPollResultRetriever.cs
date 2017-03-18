using SelectedHelpFromAudienceJokerCommand = Commands.Jokers.Selected.SelectedHelpFromAudienceJokerCommand;

namespace Jokers.Retrievers
{

    using Commands;

    using Interfaces.Network.NetworkManager;

    public class AudienceAnswerPollResultRetriever : AnswerPollResultRetriever
    {
        public const int DefaultTime = 5;

        public AudienceAnswerPollResultRetriever(IClientNetworkManager networkManager, int receiveSettingsTimeoutInSeconds = DefaultTime)
            : base(networkManager, receiveSettingsTimeoutInSeconds)
        {
        }
        
        public override void Activate()
        {
            var command = NetworkCommandData.From<SelectedHelpFromAudienceJokerCommand>();
            base.networkManager.SendServerCommand(command);

            base.Activate();
        }
    }
}