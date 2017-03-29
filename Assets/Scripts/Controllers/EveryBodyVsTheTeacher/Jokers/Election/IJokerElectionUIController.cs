namespace Assets.Scripts.Interfaces.Controllers.Jokers
{

    using UnityEngine;

    public interface IJokerElectionUIController
    {
        void SetJokerImage(Sprite sprite);

        void AddThumbsUp();

        void AddThumbsDown();
    }

}