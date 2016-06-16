using UnityEngine;
using System.Collections;
using System;

public class PlayerScore
{
    public PlayerScore(string playerName, int score)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            throw new ArgumentNullException("playerName");
        }

        if (score < 0)
        {
            throw new ArgumentNullException("score");
        }

        this.PlayerName = playerName;
        this.Score = score;
    }

    public string PlayerName
    {
        get;
        private set;
    }

    public int Score
    {
        get;
        private set;
    }
}
