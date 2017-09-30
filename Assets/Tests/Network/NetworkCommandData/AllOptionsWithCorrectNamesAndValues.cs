using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using System.Collections.Generic;

namespace Tests.Network.NetworkCommandData
{

    public class AllOptionsWithCorrectNamesAndValues : MonoBehaviour
    {
        void Start()
        {
            var commandData = new NetworkCommandDataClass("123");
            commandData.AddOption("Option1", "Value1");
            commandData.AddOption("Option2", "Value2");

            var options = commandData.Options;

            if (options.ContainsKey("Option1") &&
                options["Option1"] == "Value1" &&
                options.ContainsKey("Option2") &&
                options["Option2"] == "Value2")
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
   