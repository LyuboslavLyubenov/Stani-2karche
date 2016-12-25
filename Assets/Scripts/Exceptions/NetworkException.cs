using System;

namespace Assets.Scripts.Exceptions
{

    public class NetworkException : Exception
    {
        public NetworkException(byte errorN)
        {
            this.ErrorN = errorN;
        }

        public byte ErrorN
        {
            get;
            private set;
        }

    }

}
