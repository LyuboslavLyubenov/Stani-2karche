using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using System;

public class LeaderboardSerializer : MonoBehaviour
{
    const string FilePath = "LevelData/";
    const string FileName = "rating.csv";

    List<PlayerScore> leaderboard = new List<PlayerScore>();

    public IList<PlayerScore> Leaderboard
    {
        get
        {
            return leaderboard;
        }
    }

    void Start()
    {
        var sr = new StreamReader(FilePath + FileName);

        while (true)
        {
            var line = sr.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            var playerScoreData = line.Split(',');
            var name = playerScoreData[0];
            var score = int.Parse(playerScoreData[1]);
            var playerScore = new PlayerScore(name, score);

            leaderboard.Add(playerScore);
        }

        sr.Close();

        leaderboard = leaderboard.OrderByDescending(ps => ps.Score).ToList();
    }

    public void SetPlayerScore(PlayerScore playerScore)
    {
        throw new NotImplementedException();
    }
}
