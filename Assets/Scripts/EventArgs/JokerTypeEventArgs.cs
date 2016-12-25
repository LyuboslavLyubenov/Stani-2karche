using System;

namespace Assets.Scripts.EventArgs
{

    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class JokerTypeEventArgs : EventArgs
    {
        public Type JokerType
        {
            get;
            private set;
        }

        public JokerTypeEventArgs(Type jokerType)
        {
            JokerUtils.ValidateJokerType(jokerType);
            this.JokerType = jokerType;
        }
    }

}