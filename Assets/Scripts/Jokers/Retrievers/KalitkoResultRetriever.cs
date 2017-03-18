using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Jokers.Retrievers
{

    using Commands;

    using Interfaces.Network.NetworkManager;

    public class KalitkoResultRetriever : AnswerPollResultRetriever
    {
        public KalitkoResultRetriever(IClientNetworkManager networkManager, int receiveSettingsTimeoutInSeconds)
            : base(networkManager, receiveSettingsTimeoutInSeconds)
        {
        }

        public override void Activate()
        {
            var command = NetworkCommandData.From<SelectedKalitkoJokerCommand>();
            this.networkManager.SendServerCommand(command);

            base.Activate();
        }
    }
}
