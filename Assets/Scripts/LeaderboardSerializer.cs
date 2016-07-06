using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using CielaSpike;
using System.Threading;

/// <summary>
/// Used to load leaderboard file data
/// </summary>
public class LeaderboardSerializer : MonoBehaviour
{
    const string FilePath = "LevelData\\теми";
    const string FileName = "Rating.csv";

    public string LevelCategory = "философия";
    public bool AllowDublicates = false;

    List<PlayerScore> leaderboard = new List<PlayerScore>();
    bool loaded = false;

    public IList<PlayerScore> Leaderboard
    {
        get
        {
            if (!loaded)
            {
                return null;
            }
            else
            {
                return leaderboard;    
            }
        }
    }

    public bool Loaded
    {
        get
        {
            return loaded;
        }
    }

    string GetEndPath()
    {
        return string.Format("{0}\\{1}\\{2}", FilePath, LevelCategory, FileName);
    }

    IEnumerator LoadLeaderboardAsync()
    {
        yield return null;
        Thread.Sleep(100);

        var endPath = GetEndPath();
        var sr = new StreamReader(endPath);

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
        loaded = true;
    }

    IEnumerator SetPlayerScoreAsync(PlayerScore playerScore)
    {
        if (!loaded)
        {
            throw new Exception("Still loading score");
        }

        //TODO:
        var path = GetEndPath();
        var scores = File.ReadAllLines(path).ToList();

        int playerIndex = -1;

        if (!AllowDublicates)
        {
            for (int i = 0; i < scores.Count; i++)
            {
                var scoreParams = scores[i].Split(',');

                if (scoreParams[0] == playerScore.PlayerName)
                {
                    playerIndex = i;
                    break;
                }
            }    
        }

        var scoreDataLine = string.Format("{0},{1}", playerScore.PlayerName, playerScore.Score);

        if (playerIndex > -1)
        {
            scores[playerIndex] = scoreDataLine;
        }
        else
        {
            scores.Add(scoreDataLine);
        }

        File.WriteAllLines(path, scores.ToArray());

        yield return null;
    }

    /// <summary>
    /// Sets the player score in the leaderboard file
    /// </summary>
    public void SetPlayerScore(PlayerScore playerScore)
    {
        this.StartCoroutineAsync(SetPlayerScoreAsync(playerScore));
    }

    public void LoadDataAsync()
    {
        this.StartCoroutineAsync(LoadLeaderboardAsync());
    }
}
