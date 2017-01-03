namespace Assets.Scripts.Network.Servers
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