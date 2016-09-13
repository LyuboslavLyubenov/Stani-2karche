﻿using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class PlayerScore
{
    public PlayerScore(string playerName, int score, DateTime creationDate)
        : this(playerName, score)
    {
        this.CreationDate = creationDate.ToString();
    }

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

    public static PlayerScore CreateFrom(PlayerScore_Serializable playerScore_Serializable)
    {
        var username = playerScore_Serializable.PlayerName;
        var mark = playerScore_Serializable.Score;

        if (string.IsNullOrEmpty(playerScore_Serializable.CreationDate))
        {
            return new PlayerScore(username, mark);
        }

        var date = DateTime.Parse(playerScore_Serializable.CreationDate);
        return new PlayerScore(username, mark, date);
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

    public string CreationDate
    {
        get;
        private set;
    }
}

[Serializable]
public class PlayerScore_Serializable
{
    public string PlayerName;
    public int Score;
    public string CreationDate;

    public PlayerScore_Serializable(PlayerScore playerScore)
    {
        if (playerScore == null)
        {
            throw new ArgumentNullException("playerScore");
        }

        PlayerName = playerScore.PlayerName;
        Score = playerScore.Score;
        CreationDate = playerScore.CreationDate;
    }
}