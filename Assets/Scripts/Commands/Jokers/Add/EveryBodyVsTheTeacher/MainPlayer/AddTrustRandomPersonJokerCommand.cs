using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using TrustRandomPersonJoker = Jokers.TrustRandomPersonJoker;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{

    using Assets.Scripts.Interfaces.Controllers;

    public class AddTrustRandomPersonJokerCommand : AddJokerToMainPlayerAbstract<TrustRandomPersonJoker>
    {
        public AddTrustRandomPersonJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}
