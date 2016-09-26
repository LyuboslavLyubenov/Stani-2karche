using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Timers;
using System.Collections;

public class JokersData
{
    public EventHandler<JokerEventArgs> OnAddedJoker = delegate
    {
    };

    public EventHandler<JokerEventArgs> OnRemovedJoker = delegate
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
        OnAddedJoker(this, new JokerEventArgs(joker));
    }

    public void RemoveJoker(Type joker)
    {
        if (!availableJokers.Contains(joker))
        {
            throw new ArgumentException("Main player doesnt have this type joker");
        }

        availableJokers.Remove(joker);
        OnRemovedJoker(this, new JokerEventArgs(joker));
    }
}