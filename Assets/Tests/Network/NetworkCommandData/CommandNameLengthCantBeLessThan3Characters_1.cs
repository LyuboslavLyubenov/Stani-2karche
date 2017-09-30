using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;

namespace Tests.Network.NetworkCommandData
{

    public class CommandNameLengthCantBeLessThan3Characters_1 : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass("1  2");
        }
    }

}
   