namespace Assets.Scripts.Jokers.Retrievers
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    public class AudienceAnswerPollResultRetriever : AnswerPollResultRetriever
    {
        public AudienceAnswerPollResultRetriever(IClientNetworkManager networkManager, int receiveSettingsTimeoutInSeconds)
            : base(networkManager, receiveSettingsTimeoutInSeconds)
        {
        }
        
        public override void Activate()
        {
            var command = NetworkCommandData.From<SelectedHelpFromAudienceJokerRouterCommand>();
            base.networkManager.SendServerCommand(command);

            base.Activate();
        }
    }
}