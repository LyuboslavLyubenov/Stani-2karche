using UnityEngine;

namespace Assets.Scripts.Interfaces
{

    public interface INotificationsController
    {
        void AddNotification(Color color, string message);

        void AddNotification(Color color, string message, int disableDelayInSeconds);
    }

}
