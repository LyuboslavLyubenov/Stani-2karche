namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Interfaces.Notifications;

    using UnityEngine;

    using Interfaces;
    using Notifications;

    public class NotificationFromServerCommand : INetworkManagerCommand
    {
        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
                                                           {
                                                               { "black", Color.black },
                                                               { "white", Color.white },
                                                               { "red", Color.red },
                                                               { "green", Color.green },
                                                               { "blue", Color.blue },
                                                               { "yellow", Color.yellow },
                                                               { "clear", Color.clear },
                                                               { "cyan", Color.cyan },
                                                               { "gray", Color.gray },
                                                               { "magenta", Color.magenta }
                                                           };

        public static IList<Color> SupportedColors
        {
            get
            {
                return Colors.Values.ToList();
            }
        }

        private INotificationsController notifications;

        public NotificationFromServerCommand(INotificationsController notificationsController)
        {
            if (notificationsController == null)
            {
                throw new ArgumentNullException("notificationsController");
            }

            this.notifications = notificationsController;
        }

        public void Execute(Dictionary<string, string> commandsParamsValues)
        {
            if (!commandsParamsValues.ContainsKey("Color"))
            {
                throw new ArgumentException("Missing notification color");
            }

            if (!commandsParamsValues.ContainsKey("Message"))
            {
                throw new ArgumentException("Missing notification message");
            }

            var colorName = commandsParamsValues["Color"];
            Color notificationColor = this.ParseColor(colorName);
            var notificationMessage = commandsParamsValues["Message"];

            if (this.notifications != null)
            {
                this.notifications.AddNotification(notificationColor, notificationMessage);          
            }
        }

        private Color ParseColor(string color)
        {
            if (!Colors.ContainsKey(color))
            {
                return Colors["white"];
            }

            return Colors[color];
        }
    }
}
