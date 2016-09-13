﻿using UnityEngine;

public interface INotificationService
{
    void AddNotification(Color color, string message);

    void AddNotification(Color color, string message, int disableDelayInSeconds);
}