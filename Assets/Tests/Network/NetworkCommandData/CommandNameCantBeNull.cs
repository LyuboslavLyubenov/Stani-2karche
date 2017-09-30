using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;

namespace Tests.Network.NetworkCommandData
{

    public class CommandNameCantBeNull : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass(null);
        }
    }

}
   