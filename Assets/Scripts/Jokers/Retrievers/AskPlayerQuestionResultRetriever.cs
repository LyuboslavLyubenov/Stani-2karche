namespace Jokers.Retrievers
{

    using Commands;
    using Commands.Jokers.Selected;

    using Interfaces.Network.NetworkManager;

    public class AskPlayerQuestionResultRetriever : AskClientQuestionResultRetriever
    {
        public AskPlayerQuestionResultRetriever(IClientNetworkManager networkManager, int receiveSettingsTimeout)
            : base(networkManager, receiveSettingsTimeout)
        {
        }

        public override void Activate(int playerConnectionId)
        {
            var selected = NetworkCommandData.From<SelectedAskPlayerQuestionCommand>();
            selected.AddOption("PlayerConnectionId", playerConnectionId.ToString());
            this.networkManager.SendServerCommand(selected);
            base.Activate(playerConnectionId);
        }
    }
}