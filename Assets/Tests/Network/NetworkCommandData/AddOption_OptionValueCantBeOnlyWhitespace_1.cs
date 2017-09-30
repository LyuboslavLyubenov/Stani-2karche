using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;

namespace Tests.Network.NetworkCommandData
{

    public class AddOption_OptionValueCantBeOnlyWhitespace_1 : MonoBehaviour
    {
        void Start()
        {
            new NetworkCommandDataClass("123").AddOption("1", "\t\t\t\t\t");    
        }
    }
}
   