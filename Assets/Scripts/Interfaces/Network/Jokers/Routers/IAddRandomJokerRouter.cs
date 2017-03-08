namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    public interface IAddRandomJokerRouter : IJokerRouter
    {
        void Activate(int playerConnectionId, Type[] jokersToSelectFrom);
    }

}