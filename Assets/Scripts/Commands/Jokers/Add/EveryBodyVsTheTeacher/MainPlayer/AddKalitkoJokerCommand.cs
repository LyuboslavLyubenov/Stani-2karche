using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using MainPlayerKalitkoJoker = Jokers.Kalitko.MainPlayerKalitkoJoker;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{

    using Assets.Scripts.Interfaces.Controllers;

    public class AddKalitkoJokerCommand : AddJokerToMainPlayerAbstract<MainPlayerKalitkoJoker>
    {
        public AddKalitkoJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController, networkManager)
        {
        }
    }
}
