namespace Assets.Scripts.Interfaces.Controllers
{

    using System;

    using UnityEngine;

    public interface IJokerElectionUIController
    {
        event EventHandler OnVotedFor;
        event EventHandler OnVotedAgainst;

        void SetJoker(IJoker joker);

        void AddThumbsUp();

        void AddThumbsDown();
    }

}