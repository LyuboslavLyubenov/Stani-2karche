using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{

    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers;

    public class AddLittleIsBetterThanNothingJokerCommand : AddJokerToMainPlayerAbstract<LittleIsBetterThanNothingJoker>
    {
        public AddLittleIsBetterThanNothingJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}
