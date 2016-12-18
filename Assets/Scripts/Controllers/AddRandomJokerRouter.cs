using System;
using System.Linq;

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
        var jokersTypeNamesJSON = JsonHelper.ToJson<JokerType_DTO>(jokersTypeNames);
        var selectedJokerIndex = UnityEngine.Random.Range(0, jokersTypeNames.Length);
        var selectedJoker = jokersToSelectFrom[selectedJokerIndex];

        CoroutineUtils.WaitForSeconds(1f, () => jokersData.AddJoker(selectedJoker));

        var command = NetworkCommandData.From<AddRandomJokerCommand>();
        command.AddOption("JokersTypeNamesJSON", jokersTypeNamesJSON);
        command.AddOption("SelectedJokerIndex", selectedJokerIndex.ToString());

        NetworkManager.SendClientCommand(playerConnectionId, command);

        OnActivated(this, EventArgs.Empty);
    }

}