using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class CantParseIfOptionNameWithTooLargeLength : MonoBehaviour
    {
        void Start()
        {
            Int64 length = int.MaxValue;
            length++;
            NetworkCommandDataClass.Parse("10 1 " + length + " 1");
        }
    }
    
}