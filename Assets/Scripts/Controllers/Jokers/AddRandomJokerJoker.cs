using UnityEngine;
using System;

public class AddRandomJokerJoker : IJoker
{
    IJoker[] jokers;

    AvailableJokersUIController availableJokersUIController;

    public Sprite Image
    {
        get;
        private set;
    }

    public EventHandler OnSuccessfullyActivated
    {
        get;
        set;
    }

    public AddRandomJokerJoker(IJoker[] jokersToChooseFrom, AvailableJokersUIController availableJokersUIController)
    {
        if (jokersToChooseFrom == null || jokersToChooseFrom.Length <= 0)
        {
            throw new ArgumentException("There must be at least 1 joker in collection");
        }

        if (availableJokersUIController == null)
        {
            throw new ArgumentNullException("availableJokersUIController");
        }
            
        this.availableJokersUIController = availableJokersUIController;
        this.jokers = jokersToChooseFrom;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AddRandomJoker");
    }

    public void Activate()
    {
        var index = UnityEngine.Random.Range(0, jokers.Length);
        var joker = jokers[index];

        availableJokersUIController.AddJoker(joker);

        if (OnSuccessfullyActivated != null)
        {
            OnSuccessfullyActivated(this, EventArgs.Empty);
        }
    }
}
