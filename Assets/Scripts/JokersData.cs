using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public class JokersData
{
    public EventHandler<JokerTypeEventArgs> OnAddedJoker = delegate
    {
    };

    public EventHandler<JokerTypeEventArgs> OnUsedJoker = delegate
    {
    };

    public EventHandler<JokerTypeEventArgs> OnRemovedJoker = delegate
    {
    };

    List<Type> availableJokers = new List<Type>();

    public ICollection<Type> AvailableJokers
    {
        get
        {
            return availableJokers;
        }
    }

    public JokersData(ServerNetworkManager networkManager)
    {
        var jokerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
            {
                var jokerInterfaceType = typeof(IJoker);
                var isJoker = t.GetInterfaces().Contains(jokerInterfaceType);
                return isJoker;
            })
            .ToArray();

        for (int i = 0; i < jokerTypes.Length; i++)
        {
            var jokerType = jokerTypes[i];
            var jokerName = jokerType.Name;
            var executedJokerCommand = new DummyCommand();
            executedJokerCommand.OnExecuted += (sender, args) => UsedJoker(jokerType);
            networkManager.CommandsManager.AddCommand("Selected" + jokerName, executedJokerCommand);
        }
    }

    void UsedJoker(Type jokerType)
    {
        OnUsedJoker(this, new JokerTypeEventArgs(jokerType));
    }

    void _AddJoker(Type joker)
    {
        availableJokers.Add(joker);
        OnAddedJoker(this, new JokerTypeEventArgs(joker));
    }

    public void AddJoker(Type joker)
    {
        JokerUtils.ValidateJokerType(joker);
        _AddJoker(joker);
    }

    public void AddJoker<T>() where T : IJoker
    {
        var joker = typeof(T);
        _AddJoker(joker);
    }

    public void RemoveJoker(Type joker)
    {
        if (!availableJokers.Contains(joker))
        {
            throw new ArgumentException("Main player doesnt have this type joker");
        }

        availableJokers.Remove(joker);
        OnRemovedJoker(this, new JokerTypeEventArgs(joker));
    }

    public void RemoveJoker<T>()
    {
        var joker = typeof(T);
        RemoveJoker(joker);
    }
}