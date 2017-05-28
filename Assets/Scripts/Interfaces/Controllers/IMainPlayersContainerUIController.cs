namespace Interfaces.Controllers
{
    public interface IMainPlayersContainerUIController
    {
        void ShowMainPlayerRequestedGameStart(int connectionId);

        void ShowMainPlayer(int connectionId, string username);

        void HideMainPlayer(int connectionId);

        void HideAll();

        bool IsOnScreen(int connectionId);
    }
}
