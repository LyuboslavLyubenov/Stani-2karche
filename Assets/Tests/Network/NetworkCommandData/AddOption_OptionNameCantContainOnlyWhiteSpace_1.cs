using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;

namespace Tests.Network.NetworkCommandData
{

    public class AddOption_OptionNameCantContainOnlyWhiteSpace_1 : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass("123").AddOption("\t\t\t", "123");    
        }
    }
}
   