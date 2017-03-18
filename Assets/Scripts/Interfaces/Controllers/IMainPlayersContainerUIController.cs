namespace Assets.Scripts.Interfaces.Controllers
{
    public interface IMainPlayersContainerUIController
    {
        void ShowMainPlayerRequestedGameStart(int connectionId);

        void ShowMainPlayer(int connectionId, string username);

        void HideMainPlayer(int connectionId);

        bool IsOnScreen(int connectionId);
    }
}
