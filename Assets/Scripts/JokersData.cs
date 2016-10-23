using System.Collections.Generic;
using System;

public class JokersData
{
    public EventHandler<JokerTypeEventArgs> OnAddedJoker = delegate
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

    public void AddJoker(Type joker)
    {
        JokerUtils.ValidateJokerType(joker);
        availableJokers.Add(joker);
        OnAddedJoker(this, new JokerTypeEventArgs(joker));
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
}