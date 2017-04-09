namespace Controllers.EveryBodyVsTheTeacher.Jokers.Election
{

    using UnityEngine;

    public interface IJokerElectionUIController
    {
        void SetJokerImage(Sprite sprite);

        void AddThumbsUp();

        void AddThumbsDown();
    }

}