namespace Assets.Scripts.Interfaces.Controllers
{

    public interface IAudiencePlayersContainerUIController
    {
        void ShowAudiencePlayer(int connectionId, string username);

        void HideAudiencePlayer(int connectionId);

        bool IsOnScreen(int connectionId);
    }

}