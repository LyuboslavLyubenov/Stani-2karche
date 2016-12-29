using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Notifications
{

    using Assets.Scripts.Interfaces;

    public class NotificationsServiceController : MonoBehaviour, INotificationService
    {
        private static NotificationsServiceController instance;

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
                
                return instance;
            }
        }

        private const int SpaceBetweenNotifications = 20;

        private const int StartOffset = 10;

        private const int NormalDisableDelayInSeconds = 5;

        public Transform Container;

        private HashSet<GameObject> allNotifications = new HashSet<GameObject>();

        private Transform notificationElementPrefab;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.notificationElementPrefab = Resources.Load<Transform>("Prefabs/NotificationElement");
        }

        private int GetNotificationCount()
        {
            return this.Container.childCount; 
        }

        private void _AddNotification(Color color, string message, int disableDelayInSeconds)
        {
            var notification = Instantiate(this.notificationElementPrefab);
            var notificationRect = notification.GetComponent<RectTransform>();
            var notificationImage = notification.GetComponent<Image>();
            var notificationText = notification.GetComponentInChildren<Text>();

            notificationImage.color = color;
            notificationText.text = message;

            notification.SetParent(this.Container, false);

            var newY = notificationRect.sizeDelta.y + StartOffset + (this.GetNotificationCount() * (notificationRect.sizeDelta.y + SpaceBetweenNotifications));
            notificationRect.anchoredPosition = new Vector2(0, -newY);

            var notificationController = notification.GetComponent<NotificationElementController>();
            notificationController.WaitBeforeDisableSeconds = disableDelayInSeconds;

            this.allNotifications.Add(notification.gameObject);
        }

        public void AddNotification(Color color, string message)
        {
            this._AddNotification(color, message, NormalDisableDelayInSeconds);
        }

        public void AddNotification(Color color, string message, int disableDelayInSeconds)
        {
            this._AddNotification(color, message, disableDelayInSeconds);
        }
    }

}
