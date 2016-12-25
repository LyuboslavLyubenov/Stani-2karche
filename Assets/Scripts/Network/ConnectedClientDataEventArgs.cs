using System;

namespace Assets.Scripts.Network
{

    using EventArgs = System.EventArgs;

    public class ConnectedClientDataEventArgs : EventArgs
    {
        public ConnectedClientData ClientData 
        {
            get; 
            set; 
        }

        public ConnectedClientDataEventArgs(ConnectedClientData clientData)
        {
            if (clientData == null)
            {
                throw new ArgumentNullException("clientData");
            }
            
            this.ClientData = clientData;
        }
    }

}