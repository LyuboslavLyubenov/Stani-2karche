using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.Events;

public class ReceivedServerSelectedCategoryCommand : IOneTimeExecuteCommand
{
    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public bool FinishedExecution
    {
        get;
        private set;
    }

    LeaderboardSerializer leaderboard;

    LocalGameData gameData;

    public ReceivedServerSelectedCategoryCommand(LocalGameData gameData, LeaderboardSerializer leaderboard)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }
            
        if (leaderboard == null)
        {
            throw new ArgumentNullException("leaderboard");
        }
            
        this.gameData = gameData;
        this.leaderboard = leaderboard;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var category = commandsOptionsValues["Category"];

        gameData.LevelCategory = category;
        leaderboard.LevelCategory = category;

        gameData.LoadDataAsync();
        leaderboard.LoadDataAsync();
    }
}