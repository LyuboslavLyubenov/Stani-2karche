﻿namespace Interfaces.Notifications
{

    using UnityEngine.EventSystems;

    public interface INotificationElementController : IPointerUpHandler
    {
        void Dismiss();
    }
}
