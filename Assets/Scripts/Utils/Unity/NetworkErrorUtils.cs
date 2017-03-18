namespace Utils.Unity
{

    using System;

    using Localization;

    using UnityEngine;
    using UnityEngine.Networking;

    public class NetworkErrorUtils
    {
        private NetworkErrorUtils()
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


