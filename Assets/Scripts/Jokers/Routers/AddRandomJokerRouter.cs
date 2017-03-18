
namespace Jokers.Routers
{

    using System;
    using System.Linq;

    using Commands;
    using Commands.Jokers.Add;

    using DTOs;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;

    using Network;
    using Network.NetworkManagers;

    using Utils;

    using EventArgs = System.EventArgs;

    public class AddRandomJokerRouter : IAddRandomJokerRouter
    {
        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        private readonly ServerNetworkManager networkManager;
        private readonly JokersData jokersData;

        public AddRandomJokerRouter(ServerNetworkManager networkManager, JokersData jokersData)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (jokersData == null)
            {
                throw new ArgumentNullException("jokersData");
            }

            this.networkManager = networkManager;
            this.jokersData = jokersData;
        }

        public void Activate(int playerConnectionId, Type[] jokersToSelectFrom)
        {
            var jokerInterfaceType = typeof(IJoker);
            var areAllJokers = jokersToSelectFrom.All(j => j.GetInterface(jokerInterfaceType.Name) != null);

            if (!areAllJokers)
            {
                this.OnError(this, new UnhandledExceptionEventArgs(new ArgumentException("Not all are jokers"), true));
                return;
            }

            var jokersTypeNames = jokersToSelectFrom.Select(j => new JokerType_DTO() { JokerType = j.Name }).ToArray();
            var jokersTypeNamesJSON = JsonArrayUtility.ArrayToJson<JokerType_DTO>(jokersTypeNames);
            var selectedJokerIndex = UnityEngine.Random.Range(0, jokersTypeNames.Length);
            var selectedJoker = jokersToSelectFrom[selectedJokerIndex];

            var timer = TimerUtils.ExecuteAfter(1f, () => this.jokersData.AddJoker(selectedJoker));
            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
            timer.Start();

            var command = NetworkCommandData.From<AddRandomJokerCommand>();
            command.AddOption("JokersTypeNamesJSON", jokersTypeNamesJSON);
            command.AddOption("SelectedJokerIndex", selectedJokerIndex.ToString());

            this.networkManager.SendClientCommand(playerConnectionId, command);

            this.OnActivated(this, EventArgs.Empty);
        }

    }
}