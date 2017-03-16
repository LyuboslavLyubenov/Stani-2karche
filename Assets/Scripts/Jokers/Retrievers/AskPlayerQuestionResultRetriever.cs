namespace Jokers.Retrievers
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

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