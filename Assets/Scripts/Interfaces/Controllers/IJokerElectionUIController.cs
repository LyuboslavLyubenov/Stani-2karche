namespace Assets.Scripts.Interfaces.Controllers
{

    using System;

    using UnityEngine;

    public interface IJokerElectionUIController
    {
        event EventHandler OnVotedFor;
        event EventHandler OnVotedAgainst;

        void SetJokerImage(Sprite sprite);

        void AddThumbsUp();

        void AddThumbsDown();
    }

}