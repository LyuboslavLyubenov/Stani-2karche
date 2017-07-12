namespace EventArgs
{

    using System;

    using DTOs;

    using UnityEngine;

    using EventArgs = System.EventArgs;

    public class GameInfoEventArgs : EventArgs
    {
        public GameInfoEventArgs(string json)
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
