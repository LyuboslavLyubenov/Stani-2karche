using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using CielaSpike;

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

        var endPath = GetEndPath();

        if (!File.Exists(endPath))
        {
            File.Create(endPath).Close();
        }

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
            PlayerScore playerScore;

            if (playerScoreData.Length == 3)
            {
                var creationDate = DateTime.Parse(playerScoreData[2]);
                playerScore = new PlayerScore(name, score, creationDate);
            }
            else
            {
                playerScore = new PlayerScore(name, score);
            }

            leaderboard.Add(playerScore);
        }

        sr.Close();

        leaderboard = leaderboard.OrderByDescending(ps => ps.Score).ToList();
        loaded = true;
    }

    IEnumerator SavePlayerScoreAsync(PlayerScore playerScore)
    {
        if (!loaded)
        {
            throw new Exception("Still loading score");
        }
            
        var path = GetEndPath();
        var scores = File.ReadAllLines(path).ToList();

        int playerIndex = -1;

        if (!AllowDublicates)
        {
            for (int i = 0; i < scores.Count; i++)
            {
                var scoreData = scores[i].Split(',');

                if (scoreData[0] == playerScore.PlayerName)
                {
                    playerIndex = i;
                    break;
                }
            }    
        }

        var data = new List<string>();
        var propertyInfos = playerScore.GetType().GetProperties();

        for (int i = 0; i < propertyInfos.Length; i++)
        {
            var propertyValue = propertyInfos[i].GetValue(playerScore, null);
            data.Add(propertyValue.ToString());
        }

        var score = string.Join(",", data.ToArray());

        if (playerIndex > -1)
        {
            scores[playerIndex] = score;
        }
        else
        {
            scores.Add(score);
        }

        File.WriteAllLines(path, scores.ToArray());

        yield return null;
    }

    /// <summary>
    /// Sets the player score in the leaderboard file
    /// </summary>
    public void SavePlayerScore(PlayerScore playerScore)
    {
        this.StartCoroutineAsync(SavePlayerScoreAsync(playerScore));
    }

    public void LoadDataAsync()
    {
        this.StartCoroutineAsync(LoadLeaderboardAsync());
    }
}
