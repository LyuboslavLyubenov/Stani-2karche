using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class NotificationsController : MonoBehaviour
{
    const int SpaceBetweenNotifications = 10;
    const int StartOffset = 10;

    public Transform Container;

    HashSet<GameObject> allNotifications = new HashSet<GameObject>();

    Transform notificationElementPrefab;
    float notificationElementSizeY;
    RectTransform rect;

    void Start()
    {
        notificationElementPrefab = Resources.Load<Transform>("Prefabs/NotificationElement");
        rect = GetComponent<RectTransform>();

        notificationElementSizeY = notificationElementPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void AddNotification(Color color, string message)
    {
        var notification = Instantiate(notificationElementPrefab);
        var notificationRect = notification.GetComponent<RectTransform>();
        var notificationImage = notification.GetComponent<Image>();
        var notificationText = notification.GetComponentInChildren<Text>();

        notificationImage.color = color;
        notificationText.text = message;

        notification.SetParent(Container, false);

        var newY = StartOffset + (GetNotificationCount() * notificationRect.sizeDelta.y + SpaceBetweenNotifications);
        notificationRect.anchoredPosition = new Vector2(0, -newY);

        allNotifications.Add(notification.gameObject);
    }

    int GetNotificationCount()
    {
        return Container.childCount; 
    }
}
