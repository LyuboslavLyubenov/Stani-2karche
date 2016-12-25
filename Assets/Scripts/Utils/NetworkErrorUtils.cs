using System;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils
{

    using Assets.Scripts.Localization;

    public class NetworkErrorUtils
    {
        NetworkErrorUtils()
        {
        }

        public static string GetMessage(NetworkError error)
        {
            var enumName = Enum.GetName(typeof(NetworkError), error);
            return LanguagesManager.Instance.GetValue("NetworkMessages/" + enumName);
        }

        public static string GetMessage(NetworkConnectionError error)
        {
            var enumName = Enum.GetName(typeof(NetworkConnectionError), error);
            return LanguagesManager.Instance.GetValue("NetworkMessages/" + enumName);
        }
    }

}


