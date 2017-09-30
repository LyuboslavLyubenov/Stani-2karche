using System;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using UnityEngine;
using System.Linq;

namespace Tests.Network.NetworkCommandData
{

    public class CommandNameCantBeMoreThan100CharactersLong : MonoBehaviour
    {
        void Start()
        {
            var commandName = string.Join("", Enumerable.Range(1, 200).Select(n => n.ToString()).ToArray());
            new NetworkCommandDataClass(commandName);
        }
    }
}
   