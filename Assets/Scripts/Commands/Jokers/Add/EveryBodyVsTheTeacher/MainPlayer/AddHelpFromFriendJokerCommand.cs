using HelpFromFriendJoker = Jokers.HelpFromFriendJoker;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{
    using Assets.Scripts.Interfaces.Controllers;

    public class AddHelpFromFriendJokerCommand : AddJokerToMainPlayerAbstract<HelpFromFriendJoker>
    {
        public AddHelpFromFriendJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}
