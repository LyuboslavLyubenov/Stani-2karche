﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NotificationsServiceController : MonoBehaviour, INotificationService
{
    static NotificationsServiceController instance;

    public static NotificationsServiceController Instance
    {
        get
        {
            if (instance == null)
            {
                var prefab = Resources.Load<GameObject>("Prefabs/Notifications");
                var canvas = GameObject.FindObjectOfType<Canvas>().transform;
                var obj = (GameObject)Instantiate(prefab, canvas, false);
                instance = obj.GetComponent<NotificationsServiceController>();
            }

            //TODO: TEST
            return instance;
        }
    }

    const int SpaceBetweenNotifications = 20;
    const int StartOffset = 10;
    const int NormalDisableDelayInSeconds = 5;

    public Transform Container;

    HashSet<GameObject> allNotifications = new HashSet<GameObject>();

    Transform notificationElementPrefab;

    void Start()
    {
        notificationElementPrefab = Resources.Load<Transform>("Prefabs/NotificationElement");
    }

    int GetNotificationCount()
    {
        return Container.childCount; 
    }

    void _AddNotification(Color color, string message, int disableDelayInSeconds)
    {
        var notification = Instantiate(notificationElementPrefab);
        var notificationRect = notification.GetComponent<RectTransform>();
        var notificationImage = notification.GetComponent<Image>();
        var notificationText = notification.GetComponentInChildren<Text>();

        notificationImage.color = color;
        notificationText.text = message;

        notification.SetParent(Container, false);

        var newY = notificationRect.sizeDelta.y + StartOffset + (GetNotificationCount() * (notificationRect.sizeDelta.y + SpaceBetweenNotifications));
        notificationRect.anchoredPosition = new Vector2(0, -newY);

        var notificationController = notification.GetComponent<NotificationElementController>();
        notificationController.WaitBeforeDisableSeconds = disableDelayInSeconds;

        allNotifications.Add(notification.gameObject);
    }

    public void AddNotification(Color color, string message)
    {
        _AddNotification(color, message, NormalDisableDelayInSeconds);
    }

    public void AddNotification(Color color, string message, int disableDelayInSeconds)
    {
        _AddNotification(color, message, disableDelayInSeconds);
    }
}
