namespace Assets.Scripts.EventArgs
{

    using System;

    using Assets.Scripts.DTOs;

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