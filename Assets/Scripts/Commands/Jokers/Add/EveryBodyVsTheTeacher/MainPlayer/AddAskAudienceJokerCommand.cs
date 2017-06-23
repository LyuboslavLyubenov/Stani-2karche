using AskAudienceJoker = Jokers.AskAudienceJoker;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{

    using Assets.Scripts.Interfaces.Controllers;

    public class AddAskAudienceJokerCommand : AddJokerToMainPlayerAbstract<AskAudienceJoker>
    {
        public AddAskAudienceJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}
