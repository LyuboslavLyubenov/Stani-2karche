using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Timers;
using System.Collections;

public class JokersData
{
    List<Type> availableJokers = new List<Type>();

    public IEnumerable<Type> AvailableJokers
    {
        get
        {
            return availableJokers;
        }
    }

    void ValidateJoker(Type joker)
    {
        var isJoker = joker.GetInterfaces().Contains(typeof(IJoker));

        if (!isJoker)
        {
            throw new ArgumentException("Invalid joker");
        }
    }

    public void AddJoker(Type joker)
    {
        ValidateJoker(joker);
        availableJokers.Add(joker);
    }

    public void RemoveJoker(Type joker)
    {
        if (!availableJokers.Contains(joker))
        {
            throw new ArgumentException("Main player doesnt have this type joker");
        }

        availableJokers.Remove(joker);
    }
}