﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Notifications;

    public class ReceivedNotificationFromServerCommand : INetworkManagerCommand
    {
        readonly static Dictionary<string, Color> Colors = new Dictionary<string, Color>()
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

        NotificationsServiceController notificationsService;

        public ReceivedNotificationFromServerCommand(NotificationsServiceController notificationsServiceController)
        {
            if (notificationsServiceController == null)
            {
                throw new ArgumentNullException("notificationsServiceController");
            }

            this.notificationsService = notificationsServiceController;
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

            if (this.notificationsService != null)
            {
                this.notificationsService.AddNotification(notificationColor, notificationMessage);          
            }
        }

        Color ParseColor(string color)
        {
            if (!Colors.ContainsKey(color))
            {
                return Colors["white"];
            }

            return Colors[color];
        }

    }

}
