using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IQuestionUIController = Interfaces.Controllers.IQuestionUIController;

namespace Assets.Scripts.Jokers
{
    public class LittleIsBetterThanNothingJoker : DisableRandomAnswersJoker
    {
        public LittleIsBetterThanNothingJoker(
            IClientNetworkManager networkManager, 
            IQuestionUIController questionUIController)
            : base(networkManager, questionUIController)
        {
        }
    }
}
