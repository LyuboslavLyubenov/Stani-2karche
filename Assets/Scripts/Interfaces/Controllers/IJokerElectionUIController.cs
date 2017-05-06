namespace Assets.Scripts.Interfaces.Controllers
{

    using UnityEngine;

    public interface IJokerElectionUIController
    {
        void SetJokerImage(Sprite sprite);

        void AddThumbsUp();

        void AddThumbsDown();
    }

}
