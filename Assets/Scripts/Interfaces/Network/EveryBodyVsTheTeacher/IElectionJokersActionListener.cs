namespace Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher
{

    using System;

    public interface IElectionJokersActionListener
    {
        void ReceiveNotifications(Type jokerType);

        void ReceiveNotifications<T>() where T : IJoker;
    }

}
