using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{


    public class Parse_Correct : MonoBehaviour
    {
        void Start()
        {
            var commandName = "asdfghjklq";
            var optionKey = "o";
            var optionValue = "v";
            var commandData = NetworkCommandDataClass.Parse("10 1 1 1 " + commandName + optionKey + optionValue);

            if (commandData.Name == commandName &&
                commandData.Options.ContainsKey(optionKey) &&
                commandData.Options[optionKey] == optionValue)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
    }
}