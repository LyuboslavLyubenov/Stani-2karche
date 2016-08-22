using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;

public class ClientConnectingState
{
    public Action OnConnected;
    public Socket Client;
}