using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Assets.Scripts
{

    using Assets.CielaSpike.Thread_Ninja;
    using Assets.Scripts.DTOs;

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
                return !this.loaded ? null : this.leaderboard;
            }
        }

        public bool Loaded
        {
            get
            {
                return this.loaded;
            }
        }

        string GetEndPath()
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..";
            return string.Format("{0}\\{1}\\{2}\\{3}", execPath, FilePath, this.LevelCategory, FileName);
        }

        IEnumerator LoadLeaderboardAsync()
        {
            yield return null;

            var endPath = this.GetEndPath();

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

                this.leaderboard.Add(playerScore);
            }

            sr.Close();

            this.leaderboard = this.leaderboard.OrderByDescending(ps => ps.Score).ToList();
            this.loaded = true;
        }

        IEnumerator SavePlayerScoreAsync(PlayerScore playerScore)
        {
            if (!this.loaded)
            {
                throw new Exception("Still loading score");
            }
            
            var path = this.GetEndPath();
            var scores = File.ReadAllLines(path).ToList();

            int playerIndex = -1;

            if (!this.AllowDublicates)
            {
                for (int i = 0; i < scores.Count; i++)
                {
                    var scoreData = scores[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (scoreData[0] == playerScore.PlayerName && int.Parse(scoreData[1]) < playerScore.Score)
                    {
                        playerIndex = i;
                        break;
                    }
                }    
            }

            if (playerIndex == -1)
            {
                yield break;
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
            this.leaderboard.Add(playerScore);
            this.StartCoroutineAsync(this.SavePlayerScoreAsync(playerScore));
        }

        public void LoadDataAsync()
        {
            this.StartCoroutineAsync(this.LoadLeaderboardAsync());
        }
    }

}
