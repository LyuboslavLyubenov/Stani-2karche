namespace Assets.Scripts.Interfaces.Notifications
{

    using UnityEngine;

    public interface INotificationsController
    {
        void AddNotification(Color color, string message);

        void AddNotification(Color color, string message, int disableDelayInSeconds);
    }

}
