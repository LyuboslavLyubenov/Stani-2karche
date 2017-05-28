namespace Interfaces.Controllers
{

    public interface IAudiencePlayersContainerUIController
    {
        void ShowAudiencePlayer(int connectionId, string username);

        void HideAudiencePlayer(int connectionId);

        void HideAll();

        bool IsOnScreen(int connectionId);
    }

}