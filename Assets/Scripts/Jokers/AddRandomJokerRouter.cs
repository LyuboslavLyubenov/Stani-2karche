using System;
using System.Linq;

namespace Assets.Scripts.Jokers
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Jokers.Add;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    using EventArgs = System.EventArgs;

    public class AddRandomJokerRouter : ExtendedMonoBehaviour
    {
        public EventHandler OnActivated = delegate
            {
            };

        public ServerNetworkManager NetworkManager;

        public void Activate(int playerConnectionId, Type[] jokersToSelectFrom, JokersData jokersData)
        {
            var jokerInterfaceType = typeof(IJoker);
            var areAllJokers = jokersToSelectFrom.All(j => j.GetInterface(jokerInterfaceType.Name) != null);

            if (!areAllJokers)
            {
                throw new ArgumentException("Must be jokers");
            }

            var jokersTypeNames = jokersToSelectFrom.Select(j => new JokerType_DTO() { JokerType = j.Name }).ToArray();
            var jokersTypeNamesJSON = JsonArrayUtility.ArrayToJson<JokerType_DTO>(jokersTypeNames);
            var selectedJokerIndex = UnityEngine.Random.Range(0, jokersTypeNames.Length);
            var selectedJoker = jokersToSelectFrom[selectedJokerIndex];

            this.CoroutineUtils.WaitForSeconds(1f, () => jokersData.AddJoker(selectedJoker));

            var command = NetworkCommandData.From<AddRandomJokerCommand>();
            command.AddOption("JokersTypeNamesJSON", jokersTypeNamesJSON);
            command.AddOption("SelectedJokerIndex", selectedJokerIndex.ToString());

            this.NetworkManager.SendClientCommand(playerConnectionId, command);

            this.OnActivated(this, EventArgs.Empty);
        }

    }

}