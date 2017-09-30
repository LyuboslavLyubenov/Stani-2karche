using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class CantParseIfOptionNameWithNotPositiveLength : MonoBehaviour
    {
        void Start()
        {
            NetworkCommandDataClass.Parse("10 1 -1 1");
        }
    }
    
}