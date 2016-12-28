namespace Assets.Scripts.EventArgs
{
    using System;

    using UnityEngine;

    using DTOs;

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
            this.GameInfo = JsonUtility.FromJson<CreatedGameInfo_DTO>(json);
        }

        public CreatedGameInfo_DTO GameInfo
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
