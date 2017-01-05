namespace Assets.Scripts.Interfaces
{

    using System;

    public interface IGameServer
    {
        event EventHandler OnGameOver;

        bool IsGameOver
        {
            get;
        }

        void EndGame();
    }

}