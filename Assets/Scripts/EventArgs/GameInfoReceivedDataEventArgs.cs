using System;

using UnityEngine;

namespace Assets.Scripts.EventArgs
{

    using Assets.Scripts.DTOs;

    using EventArgs = System.EventArgs;

    public class GameInfoReceivedDataEventArgs : EventArgs
    {
        public GameInfoReceivedDataEventArgs(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException("json");
            }

            this.JSON = json;
            this.GameInfo = JsonUtility.FromJson<CreatedGameInfo_Serializable>(json);
        }

        public CreatedGameInfo_Serializable GameInfo
        {
            get;
            private set;
        }

        public string JSON
        {
            get;
            private set;
        }
    }

}
